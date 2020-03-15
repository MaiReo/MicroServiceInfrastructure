using System;

namespace Core.Messages.Bus
{
    public interface IMessageScope : IDisposable
    {
        object Resolve(Type type);

        void Release(IMessageHandler handler);
    }
}
