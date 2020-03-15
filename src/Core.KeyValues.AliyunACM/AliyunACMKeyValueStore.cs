using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.KeyValues;
using Core.KeyValues.AliyunACM.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Core.KeyValues.AliyunACM
{
    public class AliyunACMKeyValueStore : IKeyValueStore
    {
        private readonly AliyunACMKeyValueConfiguration _configuration;

        private readonly bool _stsEnabled;
        private readonly HttpClient _httpClient;
        private readonly string _stsEndPoint;

        private readonly ILogger _logger;
        private volatile AliyunStsResponseModel _cachedSts;

        private int _stsCaching;


        public AliyunACMKeyValueStore(
            IKeyValueConfigurationRoot configurationRoot,
            HttpClient httpClient = default,
            ILogger<AliyunACMKeyValueStore> logger = default)
        {
            _cachedSts = new AliyunStsResponseModel();
            _configuration = configurationRoot.GetConfiguration();
            _stsEnabled = _configuration.IsStsEnabled();
            if (_stsEnabled)
            {
                _stsEndPoint = AliyunStsResponseModel.HTTP_ENDPOINT + _configuration.RamRoleName;
            }
            else
            {
                _stsEndPoint = string.Empty;
            }
            _httpClient = httpClient ?? new HttpClient();
            _logger = (ILogger)logger ?? NullLogger.Instance;
        }
        public async Task<string> GetAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            var (endpoint, @namespace, dataId, group, accessKey, secretKey, stsToken) = await RequireParametersAsync(key);
            var serverIp = await RequireServerIpAsync();
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var sigStr = $"{@namespace}+{group}+{timestamp}";
            var sigStrBuffer = System.Text.Encoding.UTF8.GetBytes(sigStr);
            var hmacSha1KeyBuffer = System.Text.Encoding.UTF8.GetBytes(secretKey);
            using var sha1 = new System.Security.Cryptography.HMACSHA1(hmacSha1KeyBuffer);
            var signContentBuffer = sha1.ComputeHash(sigStrBuffer);
            var sigContent = Convert.ToBase64String(signContentBuffer);
            var requestUrl = $"http://{serverIp}:8080/diamond-server/config.co?dataId={dataId}&group={group}&tenant={@namespace}";
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            requestMessage.Headers.TryAddWithoutValidation("timeStamp", $"{timestamp}");
            requestMessage.Headers.TryAddWithoutValidation("Spas-AccessKey", accessKey);
            requestMessage.Headers.TryAddWithoutValidation("Spas-Signature", sigContent);

            if (!string.IsNullOrWhiteSpace(stsToken))
            {
                requestMessage.Headers.TryAddWithoutValidation("Spas-SecurityToken", stsToken);
            }
            using var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            var byteContent = await response.Content.ReadAsByteArrayAsync();
            //TODO 不要管返回的沙雕一样的内容类型text/html; charset=GBK
            var contentType = response.Content.Headers.ContentType.ToString();
            var stringContent = string.Empty;
            switch (contentType)
            {
                default:
                    stringContent = System.Text.Encoding.UTF8.GetString(byteContent);
                    break;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotExistException("key不存在");
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new KeyValueException("服务器返回非成功响应", stringContent);
            }
            // base64解码

            var normalizeContent = stringContent.Replace('-', '+').Replace('_', '/') + "==".Substring(0, (3 * stringContent.Length) % 4);
            var decodedContentBuffer = Convert.FromBase64String(normalizeContent);
            var decodedContent = Encoding.UTF8.GetString(decodedContentBuffer);


            return decodedContent;
            // return stringContent;
        }

        public async Task SetAsync(
            string key,
            string value,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            var encodedValueBuffer = Encoding.UTF8.GetBytes(value);
            var encodedValueString = Convert.ToBase64String(encodedValueBuffer);
            var webSafeEncodedValueString = encodedValueString.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            var (endpoint, @namespace, dataId, group, accessKey, secretKey, stsToken) = await RequireParametersAsync(key);
            var serverIp = await RequireServerIpAsync();
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var sigStr = $"{@namespace}+{group}+{timestamp}";
            var sigStrBuffer = System.Text.Encoding.UTF8.GetBytes(sigStr);
            var hmacSha1KeyBuffer = System.Text.Encoding.UTF8.GetBytes(secretKey);
            using var sha1 = new System.Security.Cryptography.HMACSHA1(hmacSha1KeyBuffer);
            var signContentBuffer = sha1.ComputeHash(sigStrBuffer);
            var sigContent = Convert.ToBase64String(signContentBuffer);
            var requestUrl = $"http://{serverIp}:8080/diamond-server/basestone.do?method=syncUpdateAll";
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            requestMessage.Headers.TryAddWithoutValidation("timeStamp", $"{timestamp}");
            requestMessage.Headers.TryAddWithoutValidation("Spas-AccessKey", accessKey);
            requestMessage.Headers.TryAddWithoutValidation("Spas-Signature", sigContent);
            if (!string.IsNullOrWhiteSpace(stsToken))
            {
                requestMessage.Headers.TryAddWithoutValidation("Spas-SecurityToken", stsToken);
            }
            var requestContentString = $"dataId={dataId}&group={group}&tenant={@namespace}&content={webSafeEncodedValueString}";
            requestMessage.Content = new StringContent(requestContentString, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            using var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            var byteContent = await response.Content.ReadAsByteArrayAsync();
            //TODO 不要管返回的沙雕一样的内容类型text/html; charset=GBK
            var contentType = response.Content.Headers.ContentType.ToString();
            var stringContent = string.Empty;
            switch (contentType)
            {
                default:
                    stringContent = System.Text.Encoding.UTF8.GetString(byteContent);
                    break;
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new KeyValueException("服务器返回非成功响应", stringContent);
            }
        }

        private async Task<string> RequireServerIpAsync()
        {
            var serverIp = string.Empty;
            try
            {
                serverIp = await _httpClient.GetStringAsync($"http://{_configuration.Endpoint}:8080/diamond-server/diamond");
            }
            catch (Exception e)
            {
                throw new KeyValueException("无法获取ACM服务器IP地址", e);
            }
            serverIp = serverIp.TrimEnd('\r', '\n', ' ');
            return serverIp;
        }

        private async Task<(string endpoint, string @namespace, string dataId, string group, string accessKey, string secretKey, string stsToken)> RequireParametersAsync(string key)
        {
            //TODO 从key搞出namespace和group和dataId
            var dataId = key;
            var @namespace = _configuration.Namespace;
            const string group = "DEFAULT_GROUP";
            if (!_stsEnabled)
            {
                return (_configuration.Endpoint, @namespace, dataId, group, _configuration.AccessKey, _configuration.SecretKey, default);
            }
            _logger.LogWarning("use sts");
            await EnsureStsUpdatedAsync();
            var sts = _cachedSts;
            return (_configuration.Endpoint, @namespace, dataId, group, sts.AccessKeyId, sts.AccessKeySecret, sts.SecurityToken);
        }

        private async Task EnsureStsUpdatedAsync()
        {
            if (!_stsEnabled)
            {
                return;
            }
            if (_cachedSts.Expiration >= DateTimeOffset.Now.AddSeconds(10))
            {
                return;
            }
            if (Interlocked.CompareExchange(ref _stsCaching, 1, 0) != 0)
            {
                return;
            }
            try
            {
                var json = await _httpClient.GetStringAsync(_stsEndPoint);
                //  JsonConvert.DeserializeObject<AliyunStsResponseModel>(json);
                _cachedSts = JsonSerializer.Deserialize<AliyunStsResponseModel>(json);
                if (_cachedSts.Code != "Success")
                {
                    throw new KeyValueException($"通过RAM获取STS失败, 响应信息的Code指示不成功, 原始数据: {json}");
                }
            }
            catch (Exception e) when (!(e is KeyValueException))
            {
                throw new KeyValueException("通过RAM获取STS失败", e);
            }
            finally
            {
                Interlocked.Exchange(ref _stsCaching, 0);
            }
        }

        public Task CopyAsync(string oldKeyPrefix, string newKeyPrefix, CancellationToken cancellationToken = default)
        {
            throw new KeyValueException("使用阿里云ACM存储的本类库尚未实现批量复制");
        }
    }
}