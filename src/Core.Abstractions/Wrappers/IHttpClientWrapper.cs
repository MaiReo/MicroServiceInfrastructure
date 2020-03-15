using System.Net.Http;

namespace Core.Wrappers
{
    /// <summary>
    /// 对<see cref="System.Net.Http.HttpClient"/>的封装
    /// </summary>
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// See <see cref="System.Net.Http.HttpClient"/>
        /// </summary>
        HttpClient HttpClient { get; }
    }
}