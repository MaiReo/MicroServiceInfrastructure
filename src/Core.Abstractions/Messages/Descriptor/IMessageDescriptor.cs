namespace Core.Messages
{
    public interface IMessageDescriptor
    {
        /// <summary>
        /// 消息话题
        /// </summary>
        string MessageTopic { get; }

        /// <summary>
        /// 消息分组
        /// </summary>
        string MessageGroup { get; }

    }
}