using System;
using Core.Messages.Bus.Internal;

namespace Core.Messages.Bus.Factories
{
    public class IocMessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly MessageHandlerDescriptor _descriptor;

        public IocMessageHandlerFactory(MessageHandlerDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public virtual IMessageHandler GetHandler(IMessageScope messageScope)
        {
            if (messageScope == null)
            {
                return null;
            }
            return messageScope.Resolve(_descriptor.HandlerType) as IMessageHandler;
        }

        [Obsolete]
        public virtual Type GetHandlerType() => _descriptor.HandlerType;

        public virtual void ReleaseHandler(IMessageScope messageScope, IMessageHandler handler)
        {
            if (handler == null)
            {
                return;
            }
            messageScope.Release(handler);
        }

        public virtual MessageHandlerDescriptor GetHandlerDescriptor() => _descriptor;
    }
}