using System;

namespace Core.Messages.Bus
{
    public interface IMessageScopeCreator
    {
        IMessageScope CreateScope(IMessage message, IRichMessageDescriptor messageDescriptor);
    }
}
