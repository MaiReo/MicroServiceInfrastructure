using Autofac;
using System;

namespace Core.Messages.Bus
{
    internal class MessageScope : IMessageScope
    {
        private readonly ILifetimeScope scope;

        public MessageScope(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public void Dispose()
        {
            scope.Dispose();
        }

        public void Release(IMessageHandler handler)
        {
            if (handler is IDisposable disposable)
            {
                scope.Disposer.AddInstanceForDisposal(disposable);
            }
        }

        public object Resolve(Type type)
        {
            return scope.Resolve(type);
        }
    }
}