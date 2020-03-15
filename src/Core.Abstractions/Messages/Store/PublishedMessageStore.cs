using Core.Messages.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Store
{
    public class PublishedMessageStore : MessageStore, IPublishedMessageStore, IMessageStore
    {
        public PublishedMessageStore(
            IPublishedMessageStorageProvider storageProvider,
            IMessageHasher messageHasher,
            IMessageConverter messageConverter) : base(storageProvider, messageHasher, messageConverter)
        {
        }

        public async ValueTask<bool> IsPublishedAsync(IMessageDescriptor descriptor, IMessage message, CancellationToken cancellationToken = default)
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
