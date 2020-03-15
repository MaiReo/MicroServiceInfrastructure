using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Exceptions
{
    [Serializable]
    public class BadGatewayException : Exception
    {
        public BadGatewayException(string message, string serviceName) : base(message)
        {
            ServiceName = serviceName;
        }

        public BadGatewayException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadGatewayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string ServiceName { get; }
    }
}
