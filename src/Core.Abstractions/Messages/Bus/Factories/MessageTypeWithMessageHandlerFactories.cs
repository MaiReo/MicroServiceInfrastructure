using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Messages.Bus.Factories
{
    public class MessageTypeWithMessageHandlerFactories
    {
        public Type MessageType { get; }

        public IReadOnlyList<IMessageHandlerFactory> MessageHandlerFactories { get; }

        public MessageTypeWithMessageHandlerFactories(Type messageType, IEnumerable<IMessageHandlerFactory> eventHandlerFactories)
        {
            MessageType = messageType;
            MessageHandlerFactories = eventHandlerFactories.ToList();
        }
    }
}
