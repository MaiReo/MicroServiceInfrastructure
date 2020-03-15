using Core.Messages.Bus.Factories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Bus.Internal
{
    public interface IMessageHandlerCaller
    {
        ValueTask<bool> CallAsync(IMessageScope scope, IMessageHandlerFactory handlerFactory, IMessage message, IRichMessageDescriptor messageDescriptor, List<Exception> exceptions = null, CancellationToken cancellationToken = default);
    }
}
