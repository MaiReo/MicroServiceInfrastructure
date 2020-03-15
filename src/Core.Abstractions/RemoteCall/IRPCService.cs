using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Core.RemoteCall
{
    public interface IRPCService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="relativeUrl"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<RPCHttpResult> CallHttpServiceAsync(string serviceName, string relativeUrl, HttpMethod method, object data = default, string contentType = "application/json", IDictionary<string, string> headers = default, CancellationToken cancellationToken = default);

        ValueTask<RPCHttpResult<TResult>> CallHttpServiceAsync<TResult>(string serviceName, string relativeUrl, HttpMethod method, object data = default, string contentType = "application/json", IDictionary<string, string> headers = default, CancellationToken cancellationToken = default) where TResult : class, new();
    }
}
