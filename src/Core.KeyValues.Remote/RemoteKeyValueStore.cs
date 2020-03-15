using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues.Remote
{
    public class RemoteKeyValueStore : IKeyValueStore
    {
        private readonly RemoteKeyValueConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public RemoteKeyValueStore(
            IKeyValueConfigurationRoot configurationRoot,
            HttpClient httpClient = default)
        {
            _configuration = configurationRoot.Configuration.OfType<RemoteKeyValueConfiguration>().LastOrDefault();
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task CopyAsync(string oldKeyPrefix, string newKeyPrefix, CancellationToken cancellationToken = default)
        {
            PreCheck();

            var bodyJsonStr = "{\"oldKeyPrefix\":\""+ oldKeyPrefix +"\", \"newKeyPrefix\":\"" + newKeyPrefix +"\"}";
            

            var requestUri = new Uri(_configuration.BaseUri, $"copy");
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(bodyJsonStr, Encoding.UTF8, "application/json")
            };
            using var respose = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!respose.IsSuccessStatusCode)
            {
                throw new KeyValueException($"复制设置值失败, 旧key: {oldKeyPrefix}, 新key: {newKeyPrefix}, 状态码: {respose.StatusCode}");
            }
        }

        public async Task<string> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            PreCheck();
            var requestUri = new Uri(_configuration.BaseUri, $"value?key={key}");
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            using var respose = await _httpClient.SendAsync(request, cancellationToken);
            if (!respose.IsSuccessStatusCode)
            {
                return default;
            }
            var buffer = await respose.Content.ReadAsByteArrayAsync();
            var str = Encoding.UTF8.GetString(buffer);
            return str;
        }

        public async Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            PreCheck();
            var requestUri = new Uri(_configuration.BaseUri, $"value?key={key}");
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(value, Encoding.UTF8, "text/plain")
            };
            using var respose = await _httpClient.SendAsync(request,HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!respose.IsSuccessStatusCode)
            {
                throw new KeyValueException($"设置值失败, key: {key}, 状态码: {respose.StatusCode}");
            }
        }

        private void PreCheck()
        {
            if (_configuration is null)
            {
                throw new KeyValueException("使用了远端的kv服务但配置尚未初始化");
            }
            if (_configuration.BaseUri is null)
            {
                throw new KeyValueException("使用了远端的kv服务但配置初始化的是空的基地址");
            }
        }
    }
}
