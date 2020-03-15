using Core.Messages.Bus.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using System.Text;

namespace Core.Messages
{
    public class DefaultMessageConverter : IMessageConverter
    {
        private readonly IMessageHandlerFactoryStore _messageHandlerFactoryStore;
        private readonly IMessageDescriptorResolver _messageTopicResolver;
        private readonly ILogger _logger;

        public DefaultMessageConverter(
            IMessageHandlerFactoryStore messageHandlerFactoryStore,
            IMessageDescriptorResolver messageTopicResolver,
            ILogger<DefaultMessageConverter> logger = null)
        {
            _messageHandlerFactoryStore = messageHandlerFactoryStore;
            _messageTopicResolver = messageTopicResolver;
            _logger = (ILogger)logger ?? NullLogger.Instance;
        }

        public IMessage Deserialize(IMessageDescriptor descriptor, byte[] message)
        {
            if (descriptor == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(descriptor.MessageTopic))
            {
                return null;
            }
            if (message == null || message.Length == 0)
            {
                return null;
            }
            var allMessageTypes = _messageHandlerFactoryStore.GetAllHandledMessageTypes();
            var type = _messageTopicResolver.Resolve(descriptor, allMessageTypes);
            if (type == null)
            {
                return null;
            }
            if (!typeof(IMessage).IsAssignableFrom(type))
            {
                return null;
            }
            var stringMessage = Encoding.UTF8.GetString(message);

            IMessage typedMessageObject = null;

            try
            {
                // typedMessageObject = JsonConvert.DeserializeObject(stringMessage, type) as IMessage;
                typedMessageObject = JsonSerializer.Deserialize(stringMessage, type) as IMessage;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, "反序列化消息失败");
            }

            return typedMessageObject;
        }

        public byte[] Serialize(IMessage message)
        {
            var stringMessage = SerializeString(message);
            if (string.IsNullOrWhiteSpace(stringMessage))
            {
                return null;
            }
            var raw = Encoding.UTF8.GetBytes(stringMessage);
            return raw;
        }

        public string SerializeString(IMessage message)
        {
            var stringMessage = "{}";
            if (message != null)
            {
                try
                {
                    // stringMessage = JsonConvert.SerializeObject(message);
                    stringMessage = JsonSerializer.Serialize(message, message.GetType());
                }
                catch (System.Exception e)
                {
                    _logger.LogError(e, "序列化消息失败");
                }
            }
            return stringMessage;
        }
        public string DeSerializeString(byte[] message)
        {
            if (message == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(message);
        }
    }
}