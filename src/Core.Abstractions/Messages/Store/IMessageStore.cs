using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Store
{
    public interface IMessageStore
    {
        ValueTask StoreAsync(IMessageDescriptor messageDescriptor, IMessage message, CancellationToken cancellationToken = default);
    }
}
