using System;

namespace Core.Messages.Bus
{
    public class NullMessageScope : IMessageScope
    {
        public void Dispose()
        {
            //No action.
        }

        public void Release(IMessageHandler handler)
        {
            //No action.
        }

        public object Resolve(Type type)
        {
            return null;
        }
    }
}
