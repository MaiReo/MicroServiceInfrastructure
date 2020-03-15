using System;

namespace Core.Messages.Bus
{
    public class NullMessageScopeCreator : IMessageScopeCreator
    {
        public IMessageScope CreateScope(IMessage message, IRichMessageDescriptor messageDescriptor)
        {
            return new NullMessageScope();
        }
    }
}
