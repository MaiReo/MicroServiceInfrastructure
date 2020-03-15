using Core.Messages.Bus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Messages
{
    public class RabbitMQWrapper : IRabbitMQWrapper
    {
        private readonly ConcurrentDictionary<IMessageDescriptor, IModel> _channels;
        private ConcurrentDictionary<IMessageDescriptor, Func<IMessage, IRichMessageDescriptor, ValueTask>> _subscribers;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IMessageBusOptions _messageBusOptions;
        private readonly IMessageConverter _messageConverter;

        public ILogger Logger { get; }

        protected RabbitMQWrapper()
        {
            _channels = new ConcurrentDictionary<IMessageDescriptor, IModel>(MessageDescriptorEqualityComparer.Instance);
            _subscribers = new ConcurrentDictionary<IMessageDescriptor, Func<IMessage, IRichMessageDescriptor, ValueTask>>(MessageDescriptorEqualityComparer.Instance);
        }

        public RabbitMQWrapper(
            IRabbitMQPersistentConnection rabbitMQPersistentConnection,
            IMessageBusOptions messageBusOptions,
            IMessageConverter messageConverter,
            ILogger<RabbitMQWrapper> logger) : this()
        {
            _persistentConnection = rabbitMQPersistentConnection;
            _messageBusOptions = messageBusOptions;
            _messageConverter = messageConverter;
            Logger = (ILogger)logger ?? NullLogger.Instance;
        }

        public void Subscribe(IMessageDescriptor descriptor, Action<IMessage> handler) => Subscribe(descriptor, message =>
        {
            handler?.Invoke(message);
            return new ValueTask();
        });

        public void Subscribe(IMessageDescriptor descriptor, Action<IMessage, IRichMessageDescriptor> handler) => Subscribe(descriptor, (message, _descriptor) =>
        {
            handler?.Invoke(message, _descriptor);
            return new ValueTask();
        });

        public void Subscribe(IMessageDescriptor descriptor, Func<IMessage, ValueTask> asyncHandler) => Subscribe(descriptor, async (msg, desc) => await asyncHandler(msg));

        public void Subscribe(IMessageDescriptor descriptor, Func<IMessage, IRichMessageDescriptor, ValueTask> asyncHandler)
        {
            Logger.LogInformation($"Subscribing: Exchange: {descriptor.MessageGroup}, Topic: {descriptor.MessageTopic}");
            _subscribers.AddOrUpdate(descriptor, asyncHandler, (desc, oldValue) => asyncHandler);
            CreateConsumerChannel(descriptor);
        }

        public void UnSubscribe(IMessageDescriptor descriptor)
        {
            _subscribers.TryRemove(descriptor, out var value);
        }

        public void Publish(IMessageDescriptor descriptor, byte[] message)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: descriptor.MessageGroup,
                                    durable: true,
                                    type: "topic");


                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                if (descriptor is IRichMessageDescriptor rich && rich.Headers != null)
                {
                    var headers = properties.Headers = properties.Headers ?? new Dictionary<string, object>();
                    foreach (var item in rich.Headers)
                    {
                        headers.Add(item.Key, item.Value);
                    }
                }

                channel.BasicPublish(exchange: descriptor.MessageGroup,
                                 routingKey: descriptor.MessageTopic,
                                 mandatory: true,
                                 basicProperties: properties,
                                 body: message);
            }
        }

        public ValueTask PublishAsync(IMessageDescriptor descriptor, byte[] message)
        {
            return new ValueTask(Task.Run(() => Publish(descriptor, message)));
        }

        private IModel CreateConsumerChannel(IMessageDescriptor descriptor)
        {
            const string MODE = "topic";
            if (_channels.TryGetValue(descriptor, out var exists))
            {
                Logger.LogWarning($"CreateConsumerChannel: Channel is already created");
                return exists;
            }

            var channel = _persistentConnection.CreateModel();

            void onModelShutdown(object sender, ShutdownEventArgs eventArgs)
            {
                var model = sender as IModel;
                
                switch (eventArgs.Initiator)
                {
                    case ShutdownInitiator.Application:
                        if (eventArgs.ReplyCode == 200)
                        {
                            model.ModelShutdown -= onModelShutdown;
                            Logger.LogInformation($"CreateConsumerChannel: channel { model.ChannelNumber} shutdown by app.");
                            _channels.TryRemove(descriptor, out var _);
                            return;
                        }
                        break;
                    case ShutdownInitiator.Library:
                        if (eventArgs.ReplyCode == 541) //Unhandled Exception
                        {
                            var shutdownReason = (eventArgs.Cause as Exception).Message;
                            Logger.LogWarning($"CreateConsumerChannel: channel { model.ChannelNumber} shutdown, Initiator: {eventArgs.Initiator}, caused by :{(eventArgs.Cause as Exception).Message}");
                        }
                        break;
                    case ShutdownInitiator.Peer:
                        break;
                    default:
                        break;
                }
            }

            channel.ModelShutdown += onModelShutdown;

            channel.ExchangeDeclare(exchange: descriptor.MessageGroup,
                                 durable: true,
                                 type: MODE);

            var queueName = _messageBusOptions.QueuePerConsumer != false
                            ? string.Concat(_messageBusOptions.QueueName, "-", descriptor.MessageTopic)
                            : _messageBusOptions.QueueName;

            channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: queueName,
                                  exchange: descriptor.MessageGroup,
                                  routingKey: descriptor.MessageTopic);


            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Shutdown += ConsumerShutdown;

            consumer.Received += ConsumerMessageReceived;

            var consumerTag = channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);

            Logger.LogInformation($"CreateConsumerChannel: Exchange: {descriptor.MessageGroup}, RoutingKey: {descriptor.MessageTopic}, Mode:{MODE}, Queue: {queueName}, ConsumerTag: {consumerTag}");
            _channels.TryAdd(descriptor, channel);
            return channel;
        }

        private Task ConsumerShutdown(object sender, ShutdownEventArgs eventArgs)
        {
            var consumer = sender as AsyncEventingBasicConsumer;
            switch (eventArgs.Initiator)
            {
                case ShutdownInitiator.Application:
                    if (eventArgs.ReplyCode == 200)
                    {
                        consumer.Shutdown -= ConsumerShutdown;
                        consumer.Received -= ConsumerMessageReceived;
                        Logger.LogInformation($"Consumer {consumer.ConsumerTag} shutdown by app.");
                        return Task.CompletedTask;
                    }
                    break;
                case ShutdownInitiator.Library:
                    if (eventArgs.ReplyCode == 541) //Unhandled Exception
                    {
                        var shutdownReason = (eventArgs.Cause as Exception).Message;
                        Logger.LogWarning($"Consumer {consumer.ConsumerTag} shutdown, code: {eventArgs.ReplyCode}, cause by: {shutdownReason}");
                    }
                    break;
                case ShutdownInitiator.Peer:
                    //TODO:What should we do?
                    break;
                default:
                    break;
            }
            
            return Task.CompletedTask;
        }


        private async Task ConsumerMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            var consumer = sender as IBasicConsumer;
            var descriptor = new RichMessageDescriptor(eventArgs.Body,
                eventArgs.Exchange, eventArgs.RoutingKey, eventArgs.Redelivered, eventArgs.BasicProperties?.ContentEncoding, eventArgs.BasicProperties?.ContentType, eventArgs.BasicProperties?.MessageId, eventArgs.BasicProperties?.Persistent, eventArgs.BasicProperties?.Headers);
            try
            {
                await ProcessMessageAsync(descriptor, eventArgs.Body);
                Logger.LogInformation($"处理消息成功, 即将Ack, Consumer: {eventArgs.ConsumerTag} Exchange: {descriptor.MessageGroup}, RoutingKey: {descriptor.MessageTopic}, Redelivered: {descriptor.Redelivered}");
                consumer.Model.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"处理消息出错, 即将Nack并设置reueue为true, Consumer: {eventArgs.ConsumerTag} Exchange: {descriptor.MessageGroup}, RoutingKey: {descriptor.MessageTopic}, Redelivered: {descriptor.Redelivered}");
                consumer.Model.BasicNack(eventArgs.DeliveryTag, multiple: false, true);
            }

        }

        private async ValueTask ProcessMessageAsync(
            IRichMessageDescriptor descriptor, byte[] message)
        {
            Logger.LogInformation($"ProcessMessageAsync: Exchange: {descriptor.MessageGroup}, Topic: {descriptor.MessageTopic}");
            if (!_subscribers.TryGetValue(descriptor, out var func))
            {
                Logger.LogWarning($"ProcessMessageAsync: Subscriber not exists");
                return;
            }
            var typedMessage = _messageConverter.Deserialize(descriptor, message);
            if (typedMessage == null)
            {
                Logger.LogWarning($"ProcessMessageAsync: Cannot convert message to a typed message");
                return;
            }
            await func(typedMessage, descriptor);
        }
    }
}