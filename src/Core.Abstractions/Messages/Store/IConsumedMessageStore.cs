using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Store
{
    public interface IConsumedMessageStore : IMessageStore
    {
        ValueTask<bool> IsConsumedAsync(IMessageDescriptor descriptor, IMessage message, CancellationToken cancellationToken = default);
    }
}
