using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Store
{
    public interface IMessageStorageProvider
    {
        ValueTask SaveAsync(MessageModel model, CancellationToken cancellationToken = default);

        ValueTask<MessageModel> FindAsync(string hash, CancellationToken cancellationToken = default);
    }
}
