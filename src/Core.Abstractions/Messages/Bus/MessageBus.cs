using Core.Messages.Bus.Factories;
using Core.Messages.Bus.Internal;
using Core.Messages.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Bus
{
    public class MessageBus : IMessageBus
    {

        private readonly IMessageHandlerFactoryStore _messageHandlerFactoryStore;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageScopeCreator _scopeCreator;
        private readonly IMessageHandlerCaller _messageHandlerCaller;
        private readonly ILogger _logger;

        public MessageBus(
            IMessageHandlerFactoryStore messageHandlerFactoryStore,
            IMessagePublisher messagePublisher,
            IMessageScopeCreator scopeCreator,
            IMessageHandlerCaller messageHandlerCaller = null,
            ILogger<MessageBus> logger = null)
        {
            _messageHandlerFactoryStore = messageHandlerFactoryStore;
            _messagePublisher = messagePublisher;
            _scopeCreator = scopeCreator;
            _messageHandlerCaller = messageHandlerCaller ?? ReflectionMessageHandlerCaller.Instance;
            _logger = (ILogger)logger ?? NullLogger.Instance;
        }

        /// <inheritdoc/>
        public virtual async ValueTask PublishAsync<T>(T message) where T : IMessage
        {
            await _messagePublisher.PublishAsync(message);
        }

        public virtual async ValueTask OnMessageReceivedAsync(IMessage message, IRichMessageDescriptor descriptor)
        {
            if (message is null)
            {
                return;
            }
            using (var scope = _scopeCreator.CreateScope(message, descriptor))
            {
                var messageStore = scope.Resolve(typeof(IConsumedMessageStore)) as IConsumedMessageStore;

                if (await messageStore.IsConsumedAsync(descriptor, message))
                {
                    _logger.LogWarning($"[{descriptor.MessageGroup}][{descriptor.MessageTopic}][{descriptor.MessageId}]Message already consumed.");
                    return;
                }
                await ProcessMessageAsync(scope, message.GetType(), message, descriptor);
                await messageStore.StoreAsync(descriptor, message);
            }
        }

        protected async ValueTask ProcessMessageAsync(IMessageScope scope, Type messageType, IMessage message, IRichMessageDescriptor descriptor)
        {
            var exceptions = new List<Exception>();

            await new SynchronizationContextRemover();

            foreach (var handlerFactories in _messageHandlerFactoryStore.GetHandlerFactories(messageType).ToList())
            {
                foreach (var handlerFactory in handlerFactories.MessageHandlerFactories)
                {
                    var isCallSuccess = await _messageHandlerCaller.CallAsync(scope, handlerFactory, message, descriptor, exceptions);

                    if (!isCallSuccess)
                    {
                        var errorMessage = $"Call message handler error";
                        exceptions.Add(new MessageBusException(errorMessage));
                    }
                }
            }

            //Implements generic argument inheritance. See IMessageWithInheritableGenericArgument
            if (messageType.IsGenericType &&
                messageType.GenericTypeArguments.Length == 1 &&
                typeof(IMessageWithInheritableGenericArgument).IsAssignableFrom(messageType))
            {
                var genericArg = messageType.GetGenericArguments()[0];
                var baseArg = genericArg.BaseType;
                if (baseArg != null)
                {
                    var baseMessageType = messageType.GetGenericTypeDefinition().MakeGenericType(baseArg);
                    var constructorArgs = ((IMessageWithInheritableGenericArgument)messageType).GetConstructorArgs();
                    //TODO: Use Expression Tree instead / or do Inheritable Abstractions
                    var baseMessage = (IMessage)Activator.CreateInstance(baseMessageType, constructorArgs);
                    await ProcessMessageAsync(scope, baseMessageType, baseMessage, descriptor);
                }
            }

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
                }

                throw new AggregateException("More than one error has occurred while handling the message: " + messageType, exceptions);
            }
        }


        

        public IEnumerable<Type> GetAllHandledMessageTypes()
        {
            return _messageHandlerFactoryStore.GetAllHandledMessageTypes();
        }

        public IDisposable Register(Type messageType, IMessageHandlerFactory factory)
        {
            return _messageHandlerFactoryStore.Register(messageType, factory);
        }

        public void Unregister(Type messageType, IMessageHandlerFactory factory)
        {
            _messageHandlerFactoryStore.Unregister(messageType, factory);
        }

        // Reference from
        // https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/src/Abp/Events/Bus/EventBus.cs
        // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
        private struct SynchronizationContextRemover : INotifyCompletion
        {
            public bool IsCompleted
            {
                get { return SynchronizationContext.Current == null; }
            }

            public void OnCompleted(Action continuation)
            {
                var prevContext = SynchronizationContext.Current;
                try
                {
                    SynchronizationContext.SetSynchronizationContext(null);
                    continuation();
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(prevContext);
                }
            }

            public SynchronizationContextRemover GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
            }
        }


    }
}
