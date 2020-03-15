using System.Threading.Tasks;

namespace Core.Messages
{
    /// <summary>
    /// 消息发送者包装
    /// </summary>
    public interface IMessagePublisherWrapper
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageWrapper">包装后的消息</param>
        void Publish(IMessageWrapper messageWrapper);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageWrapper">包装后的消息</param>
        ValueTask PublishAsync(IMessageWrapper messageWrapper);
    }
}