using System.Net.Http;

namespace Core.Wrappers
{
    /// <summary>
    /// 对<see cref="System.Net.Http.HttpClient"/>的封装
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="httpMessageHandler"></param>
        public HttpClientWrapper(HttpMessageHandler httpMessageHandler)
        {
            if (httpMessageHandler != null)
            {
                this.HttpClient = new HttpClient(httpMessageHandler, false);
            }
            else
            {
                this.HttpClient = new HttpClient();
            }
        }

        /// <summary>
        /// See <see cref="System.Net.Http.HttpClient"/>
        /// </summary>
        public HttpClient HttpClient { get; }
    }
}