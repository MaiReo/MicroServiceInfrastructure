using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Core.RemoteCall
{
    public class NullRPCService : IRPCService
    {
        public ValueTask<RPCHttpResult> CallHttpServiceAsync(string serviceName, string relativeUrl, HttpMethod method, object data = null, string contentType = "application/json", IDictionary<string, string> headers = null, CancellationToken cancellationToken = default)
        {
            return new ValueTask<RPCHttpResult>();
        }

        public ValueTask<RPCHttpResult<TResult>> CallHttpServiceAsync<TResult>(string serviceName, string relativeUrl, HttpMethod method, object data = null, string contentType = "application/json", IDictionary<string, string> headers = null, CancellationToken cancellationToken = default) where TResult : class, new()
        {
            return new ValueTask<RPCHttpResult<TResult>>();
        }

        public static NullRPCService Instance => new NullRPCService();
    }
}
