using System;

namespace Core.Messages
{
    /// <summary>
    /// 消息话题
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MessageDescriptionAttribute : Attribute, IMessageDescriptorProvider
    {

        /// <summary>
        /// 消息分组
        /// </summary>
        public string MessageGroup { get; }

        /// <summary>
        /// 消息话题
        /// </summary>
        public string MessageTopic { get; }

        /// <summary>
        /// 使用小写
        /// </summary>
        public bool UseLowerCase { get; set; }

        public MessageDescriptionAttribute(string messageGroup = null, string messageTopic = null)
        {
            this.MessageTopic = messageTopic;
            this.MessageGroup = messageGroup;
            this.UseLowerCase = true;
        }

        public IMessageDescriptor GetMessageDescriptor(string defaultGroup, string defaultTopic)
        {
            var group = this.MessageGroup ?? defaultGroup;
            var topic = this.MessageTopic ?? defaultTopic;
            if (UseLowerCase)
            {
                topic = topic?.ToLowerInvariant();
            }
            return new MessageDescriptor(group, topic);
        }
    }
}