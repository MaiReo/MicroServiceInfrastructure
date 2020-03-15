using System;
using System.Collections.Generic;

namespace Core.Messages
{
    public interface IMessageDescriptorResolver
    {
        IMessageDescriptor Resolve<T>(T message) where T : IMessage;

        IMessageDescriptor Resolve(Type messageType);

        Type Resolve(IMessageDescriptor descriptor, IEnumerable<Type> types);
    }
}