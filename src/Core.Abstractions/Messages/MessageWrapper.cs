namespace Core.Messages
{
    /// <summary>
    /// 消息包装
    /// </summary>
    public class MessageWrapper : IMessageWrapper
    {
        /// <summary>
        /// 消息体
        /// </summary>
        public IMessage Message { get; }
        public IMessageDescriptor Descriptor { get; }

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="message">消息</param>
        public MessageWrapper(IMessageDescriptor descriptor, IMessage message)
        {
            this.Descriptor = descriptor;
            this.Message = message;
        }
    }
}