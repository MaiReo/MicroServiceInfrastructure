using Core.Exceptions;
using Core.ServiceDiscovery;
using Core.Utilities;
using Core.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Core.RemoteCall
{
    public class RPCService : IRPCService
    {
        public IServiceDiscoveryHelper ServiceDiscoveryHelper { get; set; }

        protected virtual IHttpClientWrapper HttpClientWrapper { get; }

        public RPCService(IHttpClientWrapper httpClientWrapper)
        {
            HttpClientWrapper = httpClientWrapper;
            ServiceDiscoveryHelper = NullServiceDiscoveryHelper.Instance;
        }

        public async ValueTask<RPCHttpResult> CallHttpServiceAsync(string serviceName, string relativeUrl, HttpMethod method, object data = default, string contentType = "application/json", IDictionary<string, string> headers = default, CancellationToken cancellationToken = default)
        {
            RPCHttpResult result = default;
            HttpRequestMessage httpRequestMessage = default;
            HttpResponseMessage httpResponseMessage = default;
            IDisposableModel<HttpRequestMessage> addRef = default;
            try
            {
                addRef = await CreateHttpRequest(serviceName, relativeUrl, method, data, contentType, headers, cancellationToken);
                httpRequestMessage = addRef.Model;
                httpResponseMessage = await HttpClientWrapper.HttpClient.SendAsync(httpRequestMessage, cancellationToken);
                if (httpResponseMessage.Content == null)
                {
                    result = new RPCHttpResult(httpResponseMessage.StatusCode, default, default);
                }
                else
                {
                    var bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                    result = new RPCHttpResult(httpResponseMessage.StatusCode, bytes, httpResponseMessage.Content.Headers?.ContentType?.MediaType);
                }
            }
            catch (NotSupportedException e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }
            catch (Exception e) when (!(e is BadGatewayException))
            {
                result = new RPCHttpResult(System.Net.HttpStatusCode.InternalServerError, default, default, e);
            }
            finally
            {
                httpResponseMessage?.Content?.Dispose();
                httpResponseMessage?.Dispose();
                httpRequestMessage?.Content?.Dispose();
                httpRequestMessage?.Dispose();
                addRef?.Dispose();
            }
            return result;
        }

        public async ValueTask<RPCHttpResult<TResult>> CallHttpServiceAsync<TResult>(string serviceName, string relativeUrl, HttpMethod method, object data = default, string contentType = "application/json", IDictionary<string, string> headers = default, CancellationToken cancellationToken = default) where TResult : class, new()
        {
            var rawResult = await CallHttpServiceAsync(serviceName, relativeUrl, method, data, contentType, headers, cancellationToken);

            var httpResult = new RPCHttpResult<TResult>(rawResult.StatusCode, rawResult.Exception);

            if (rawResult.Exception != default)
            {
                return httpResult;
            }
            if (rawResult.Result == default)
            {
                return httpResult;
            }
            TResult result = default;
            using var stream = new System.IO.MemoryStream(rawResult.Result);
            try
            {

                result = await JsonSerializer.DeserializeAsync<TResult>(stream, cancellationToken: cancellationToken);
            }
            catch
            {
                //TODO
                //Logger.Error
            }
            httpResult.Result = result;
            return httpResult;
        }


        private async ValueTask<IDisposableModel<HttpRequestMessage>> CreateHttpRequest(string serviceName, string relativeUrl, HttpMethod method, object data = default, string contentType = "application/json", IDictionary<string, string> headers = default, CancellationToken cancellationToken = default)
        {
            method = method ?? HttpMethod.Get;
            contentType = contentType ?? "application/json";
            var serviceHostAndPortModel = await ServiceDiscoveryHelper.GetServiceBasePathAndAddRefAsync(serviceName, cancellationToken: cancellationToken);
            var serviceHostAndPort = serviceHostAndPortModel.Model;
            var requestUri = $"{serviceHostAndPort}{relativeUrl}";
            var httpRequestMessage = new HttpRequestMessage(method, requestUri);
            httpRequestMessage.Headers.Accept.Clear();
            httpRequestMessage.Headers.Accept.ParseAdd(contentType);
            httpRequestMessage.Headers.AcceptCharset.Clear();
            httpRequestMessage.Headers.AcceptCharset.ParseAdd("utf-8");
            HttpContent httpContent = default;

            if (HttpMethod.Get != method && HttpMethod.Delete != method && data != default)
            {
                if (contentType != "application/json")
                {
                    throw new NotSupportedException("当前仅支持json");
                }
                // var jsonString = JsonConvert.SerializeObject(data);
                // httpContent = new StringContent(jsonString, Encoding.UTF8, contentType);
                var jsonBuffer = JsonSerializer.SerializeToUtf8Bytes(data);
                httpContent = new ByteArrayContent(jsonBuffer);
                httpContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
            }
            if (httpContent != default)
            {
                httpRequestMessage.Content = httpContent;
            }
            if (headers?.Any() == true)
            {
                foreach (var header in headers)
                {
                    httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            return new DelegateDisposableModel<HttpRequestMessage>(httpRequestMessage, () => serviceHostAndPortModel.Dispose());
        }
    }
}
