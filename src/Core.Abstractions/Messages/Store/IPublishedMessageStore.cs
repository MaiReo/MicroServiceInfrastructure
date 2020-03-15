using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Store
{
    public interface IPublishedMessageStore : IMessageStore
    {
        ValueTask<bool> IsPublishedAsync(IMessageDescriptor descriptor, IMessage message, CancellationToken cancellationToken = default);
    }
}
