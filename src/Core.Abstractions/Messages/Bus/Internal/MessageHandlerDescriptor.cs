using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messages.Bus.Internal
{
    public struct MessageHandlerDescriptor
    {
        public MessageHandlerDescriptor(Type handlerType, Type messageType, bool isAsync = false, bool isRich = false)
        {
            HandlerType = handlerType;
            MessageType = messageType;
            IsAsync = isAsync;
            IsRich = isRich;
        }

        public bool IsAsync { get; }

        public bool IsRich { get; }

        public Type MessageType { get; }

        public Type HandlerType { get; }
    }
}
