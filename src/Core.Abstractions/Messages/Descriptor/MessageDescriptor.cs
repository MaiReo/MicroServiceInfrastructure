namespace Core.Messages
{
    public class MessageDescriptor : IMessageDescriptor
    {
        public MessageDescriptor(string messageGroup, string messageTopic)
        {
            this.MessageGroup = messageGroup;
            this.MessageTopic = messageTopic;
        }

        public string MessageTopic { get; }

        public string MessageGroup { get; }
    }
}