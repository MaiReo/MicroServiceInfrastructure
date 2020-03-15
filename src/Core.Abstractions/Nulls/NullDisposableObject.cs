using System;

namespace Core.Messages.Bus
{
    internal class NullDisposableObject : IDisposable
    {
        public void Dispose()
        {
            // No Actions.
        }
    }
}