using System;
using System.Runtime.Serialization;

namespace Core.Messages.Bus
{
    [Serializable]
    public class MessageBusException : Exception
    {
        public MessageBusException()
        {
        }

        public MessageBusException(string message) : base(message)
        {
        }

        public MessageBusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MessageBusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}