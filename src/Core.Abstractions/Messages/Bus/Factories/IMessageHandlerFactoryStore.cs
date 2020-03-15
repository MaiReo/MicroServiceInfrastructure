using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messages.Bus.Factories
{
    public interface IMessageHandlerFactoryStore
    {
        IReadOnlyList<Type> GetAllHandledMessageTypes();

        IEnumerable<MessageTypeWithMessageHandlerFactories> GetHandlerFactories(Type messageType);

        IDisposable Register(Type messageType, IMessageHandlerFactory factory);

        void Unregister(Type messageType, IMessageHandlerFactory factory);
    }
}
