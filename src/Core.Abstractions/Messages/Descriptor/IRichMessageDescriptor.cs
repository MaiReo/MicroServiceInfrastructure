using System.Collections.Generic;

namespace Core.Messages
{
    public interface IRichMessageDescriptor : IMessageDescriptor
    {
        /// <summary>
        /// 是否是第二次接收
        /// </summary>
        bool Redelivered { get; }

        string ContentEncoding { get; }

        string ContentType { get; }

        string MessageId { get; }

        bool? Persistent { get; }

        IDictionary<string, object> Headers { get; }

        byte[] Raw { get; }

    }
}