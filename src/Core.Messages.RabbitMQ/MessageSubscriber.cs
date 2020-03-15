using Core.Messages.Bus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;

namespace Core.Messages
{
    public class MessageSubscriber : IMessageSubscriber
    {
        private readonly IMessageBus _messageBus;
        private readonly IRabbitMQWrapper _rabbitMQWrapper;
        private readonly IMessageDescriptorResolver _messageTopicResolver;

        private bool _isAutoSubscribed;

        public MessageSubscriber(IMessageBus messageBus,
            IRabbitMQWrapper rabbitMQWrapper,
            IMessageDescriptorResolver messageTopicResolver,
            ILogger<MessageSubscriber> logger)
        {
            _messageBus = messageBus;
            _rabbitMQWrapper = rabbitMQWrapper;
            _messageTopicResolver = messageTopicResolver;
            Logger = (ILogger)logger ?? NullLogger.Instance;
        }

        public ILogger Logger { get; }

        public void AutoSubscribe()
        {
            if (_isAutoSubscribed)
            {
                throw new System.InvalidOperationException("already automatic subscribed");
            }
            var messageTypes = _messageBus.GetAllHandledMessageTypes().ToList();

            foreach (var messageType in messageTypes)
            {
                var descriptor = _messageTopicResolver.Resolve(messageType);
                Logger.LogInformation($"AutoSubscribe: Found messageType:{messageType}, topic name: {descriptor?.MessageTopic}, group name: {descriptor?.MessageGroup}");
                _rabbitMQWrapper.Subscribe(descriptor, async (msg, _descriptor) => await _messageBus.OnMessageReceivedAsync(msg, _descriptor));
            }
            _isAutoSubscribed = true;

        }
    }
}