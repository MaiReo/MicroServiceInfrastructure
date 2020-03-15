using Core.Messages.Bus.Factories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Messages.Bus
{
    public interface IMessageBus
    {
        ValueTask PublishAsync<T>(T message) where T : IMessage;

        ValueTask OnMessageReceivedAsync(IMessage message, IRichMessageDescriptor descriptor);

        IDisposable Register(Type messageType, IMessageHandlerFactory factory);

        void Unregister(Type messageType, IMessageHandlerFactory factory);

        IEnumerable<Type> GetAllHandledMessageTypes();
    }
}