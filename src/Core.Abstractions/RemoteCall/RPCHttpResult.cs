using System;
using System.Net;

namespace Core.RemoteCall
{
    public sealed class RPCHttpResult<TResult>
    {
        public RPCHttpResult(HttpStatusCode statusCode, Exception exception = default)
        {
            StatusCode = statusCode;
            Exception = exception;
        }
        public TResult Result { get; set; }

        public HttpStatusCode StatusCode { get; }

        public Exception Exception { get; }

    }
    public sealed class RPCHttpResult
    {
        public RPCHttpResult(HttpStatusCode statusCode, byte[] result, string contentType, Exception exception = default)
        {
            StatusCode = statusCode;
            Result = result;
            ContentType = contentType;
            Exception = exception;
        }

        public byte[] Result { get; set; }

        public string ContentType { get; }

        public HttpStatusCode StatusCode { get; }

        public Exception Exception { get; }
    }
}
