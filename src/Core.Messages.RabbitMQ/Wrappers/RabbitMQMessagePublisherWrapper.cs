using System.Threading.Tasks;

namespace Core.Messages
{
    public class RabbitMQMessagePublisherWrapper : IMessagePublisherWrapper
    {
        private readonly IRabbitMQWrapper _rabbitMQWrapper;
        private readonly IMessageConverter _messageConverter;

        public RabbitMQMessagePublisherWrapper(
            IRabbitMQWrapper rabbitMQWrapper,
            IMessageConverter messageConverter)
        {
            this._rabbitMQWrapper = rabbitMQWrapper;
            this._messageConverter = messageConverter;
        }

        public void Publish(IMessageWrapper messageWrapper)
        {
            var raw = _messageConverter.Serialize(messageWrapper.Message);
            if (raw == null)
            {
                return;
            }
            _rabbitMQWrapper.Publish(messageWrapper.Descriptor, raw);
        }

        public async ValueTask PublishAsync(IMessageWrapper messageWrapper)
        {
            var raw = _messageConverter.Serialize(messageWrapper.Message);
            if (raw == null)
            {
                return;
            }
            await _rabbitMQWrapper.PublishAsync(messageWrapper.Descriptor, raw);
        }
    }
}