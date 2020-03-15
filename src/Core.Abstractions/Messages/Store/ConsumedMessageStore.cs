using System.Threading;
using System.Threading.Tasks;
using Core.Messages.Utilities;

namespace Core.Messages.Store
{
    public class ConsumedMessageStore : MessageStore, IConsumedMessageStore, IMessageStore
    {
        public ConsumedMessageStore(
            IConsumedMessageStorageProvider storageProvider,
            IMessageHasher messageHasher,
            IMessageConverter messageConverter) : base(storageProvider, messageHasher, messageConverter)
        {
        }

        public async ValueTask<bool> IsConsumedAsync(IMessageDescriptor descriptor, IMessage message, CancellationToken cancellationToken = default)
        {
            if (descriptor == null)
            {
                throw new System.ArgumentNullException(nameof(descriptor));
            }
            if (message == null)
            {
                throw new System.ArgumentNullException(nameof(message));
            }
            return await base.IsExistsAsync(descriptor, message, cancellationToken);
        }
    }
}
