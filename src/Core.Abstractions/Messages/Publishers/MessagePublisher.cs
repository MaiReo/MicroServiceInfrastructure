using Core.Messages.Store;
using Core.Session;
using Core.Session.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace Core.Messages
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IMessageDescriptorResolver _messageTopicResolver;
        private readonly ICoreSessionProvider _coreSessionProvider;
        private readonly IMessagePublisherWrapper _messagePublisherWrapper;
        private readonly IPublishedMessageStore _messageStore;
        private readonly ILogger _logger;

        public MessagePublisher(
                IMessageDescriptorResolver messageTopicResolver,
                ICoreSessionProvider coreSessionProvider,
                IMessagePublisherWrapper messagePublisherWrapper,
                IPublishedMessageStore messageStore,
                ILogger<MessagePublisher> logger = null)
        {
            _messageTopicResolver = messageTopicResolver;
            _coreSessionProvider = coreSessionProvider;
            _messagePublisherWrapper = messagePublisherWrapper;
            _messageStore = messageStore;
            _logger = (ILogger)logger ?? NullLogger.Instance;
        }

        public virtual void Publish<T>(T message) where T : IMessage
        {
            var descriptor = _messageTopicResolver.Resolve(message);

            var headers = _coreSessionProvider.Session.ToHeaders();

            if (headers != null)
            {
                var rich = new RichMessageDescriptor(descriptor.MessageGroup, descriptor.MessageTopic);
                foreach (var item in headers)
                {
                    rich.Headers.Add(item.Key, item.Value);
                }
                descriptor = rich;
            }

            if (_messageStore.IsPublishedAsync(descriptor, message).GetAwaiter().GetResult())
            {
                _logger.LogWarning($"[{descriptor.MessageGroup}][{descriptor.MessageTopic}]Message has already published");
                return;
            }
            var wrapper = new MessageWrapper(descriptor, message);
            _messagePublisherWrapper.Publish(wrapper);
            _messageStore.StoreAsync(descriptor, message).GetAwaiter().GetResult();
        }

        public virtual async ValueTask PublishAsync<T>(T message) where T : IMessage
        {
            var descriptor = _messageTopicResolver.Resolve(message);
            var headers = _coreSessionProvider.Session.ToHeaders();

            if (headers != null)
            {
                var rich = new RichMessageDescriptor(descriptor.MessageGroup, descriptor.MessageTopic);
                foreach (var item in headers)
                {
                    rich.Headers.Add(item.Key, item.Value);
                }
                descriptor = rich;
            }
            if (await _messageStore.IsPublishedAsync(descriptor, message))
            {
                _logger.LogWarning($"[{descriptor.MessageGroup}][{descriptor.MessageTopic}]Message has already published");
                return;
            }
            var wrapper = new MessageWrapper(descriptor, message);
            await _messagePublisherWrapper.PublishAsync(wrapper);

            await _messageStore.StoreAsync(descriptor, message);
        }
    }
}