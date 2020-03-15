using System.Collections.Generic;

namespace Core.Messages
{
    public class RichMessageDescriptor : MessageDescriptor, IRichMessageDescriptor, IMessageDescriptor
    {
        

        public RichMessageDescriptor(string messageGroup, string messageTopic) : base(messageGroup, messageTopic)
        {
            Headers = new Dictionary<string, object>();
        }

        public RichMessageDescriptor(
            byte[] raw,
            string messageGroup,
            string messageTopic,
            bool redelivered,
            string contentEncoding,
            string contentType,
            string messageId,
            bool? persistent,
            IDictionary<string, object> headers) : this(messageGroup, messageTopic)
        {
            Raw = raw;
            Redelivered = redelivered;
            ContentEncoding = contentEncoding;
            ContentType = contentType;
            MessageId = messageId;
            Persistent = persistent;
            Headers = headers;
        }

        /// <summary>
        /// 原始数据
        /// </summary>
        public byte[] Raw { get; }

        /// <summary>
        /// 是否是第二次接收
        /// </summary>
        public bool Redelivered { get;}

        public string ContentEncoding { get; }

        public string ContentType { get; }

        public string MessageId { get;}

        public bool? Persistent { get; }

        public IDictionary<string, object> Headers { get; }

    }
}