using Core.Messages.Bus.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Messages.Bus
{
    public class NullMessageBus : IMessageBus
    {
        ValueTask IMessageBus.OnMessageReceivedAsync(IMessage message, IRichMessageDescriptor descriptor)
        {
            return new ValueTask();
        }

        ValueTask IMessageBus.PublishAsync<T>(T message)
        {
            return new ValueTask();
        }

        IDisposable IMessageBus.Register(Type messageType, IMessageHandlerFactory messageHandlerFactory)
        {
            return new NullDisposableObject();
        }


        void IMessageBus.Unregister(Type messageType, IMessageHandlerFactory factory)
        {
            // No Action.
        }

        IEnumerable<Type> IMessageBus.GetAllHandledMessageTypes()
        {
            return Enumerable.Empty<Type>();
        }

        public static NullMessageBus Instance => new NullMessageBus();
    }
}