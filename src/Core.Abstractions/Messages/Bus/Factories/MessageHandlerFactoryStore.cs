using Core.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Messages.Bus.Factories
{
    public class MessageHandlerFactoryStore  : IMessageHandlerFactoryStore
    {
        /// <summary>
        /// All registered handler factories.
        /// Key: Type of the event
        /// Value: List of handler factories
        /// </summary>
        private readonly ConcurrentDictionary<Type, ICollection<IMessageHandlerFactory>> _handlerFactories;

        public MessageHandlerFactoryStore()
        {
            _handlerFactories = new ConcurrentDictionary<Type, ICollection<IMessageHandlerFactory>>();
        }

        private ICollection<IMessageHandlerFactory> GetOrCreateHandlerFactories(Type messageType)
        {
            return _handlerFactories.GetOrAdd(messageType, (type) => new HashSet<IMessageHandlerFactory>(new MessageHandlerFactoryUniqueComparer()));
        }

        public IEnumerable<MessageTypeWithMessageHandlerFactories> GetHandlerFactories(Type messageType)
        {
            foreach (var handlerFactory in _handlerFactories.Where(hf => ShouldTriggerMessageForHandler(messageType, hf.Key)))
            {
                yield return new MessageTypeWithMessageHandlerFactories(handlerFactory.Key, handlerFactory.Value);
            }
        }

        private static bool ShouldTriggerMessageForHandler(Type messageType, Type registeredType)
        {
            //Should trigger same type
            if (registeredType == messageType)
            {
                return true;
            }

            //Should trigger for inherited types
            if (registeredType.IsAssignableFrom(messageType))
            {
                return true;
            }

            return false;
        }

        public IReadOnlyList<Type> GetAllHandledMessageTypes()
        {
            return _handlerFactories.Keys.ToList();
        }

        public IDisposable Register(Type messageType, IMessageHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(messageType)
                .Locking(factories => factories.Add(factory));
            return new MessageHandlerFactoryUnregistrar(this, messageType, factory);
        }

        public void Unregister(Type messageType, IMessageHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(messageType).Locking(factories => factories.Remove(factory));
        }
    }
}
