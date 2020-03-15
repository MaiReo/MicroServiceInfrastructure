using System.Threading.Tasks;

namespace Core.Messages
{
    /// <summary>
    /// 向消息队列发送消息
    /// </summary>
    public interface IMessagePublisher
    {
        void Publish<T>(T message) where T : IMessage;

        ValueTask PublishAsync<T>(T message) where T : IMessage;
    }
}