using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues.Json
{
    [Obsolete("Use KeyPassThroughValueWrapedAsJsonObjectKeyValueReader instead.")]
    public abstract class NestedJsonKeyValueReader : IKeyValueReader
    {
        private readonly IKeyValueStore _keyValueStore;

        public NestedJsonKeyValueReader(IKeyValueStore keyValueStore)
        {
            _keyValueStore = keyValueStore;
        }

        public virtual async Task<T> GetObjectAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var pathJsonToken = await GetJTokenAsync(key, cancellationToken);

            if (pathJsonToken == default)
            {
                return default;
            }

            if (pathJsonToken.Type == JTokenType.Null)
            {
                return default(T);
            }

            if (typeof(T) == typeof(string))
            {
                if (pathJsonToken.Type == JTokenType.String)
                {
                    return (T)(object)pathJsonToken.ToObject<string>();
                }
                else if (pathJsonToken.Type != JTokenType.Object)
                {
                    return (T)(object)pathJsonToken.ToString();
                }
            }

            var obj = pathJsonToken.ToObject<T>();
            return obj;
        }

        public virtual async Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            var pathJsonToken = await GetJTokenAsync(key, cancellationToken);

            if (pathJsonToken == default)
            {
                return default;
            }

            if (pathJsonToken.Type == JTokenType.Null)
            {
                return default;
            }

            if (pathJsonToken.Type == JTokenType.String)
            {
                return pathJsonToken.Value<string>();
            }
            else
            {
                return pathJsonToken.ToString();
            }
        }

        public virtual async Task<decimal?> GetDecimalAsync(string key, CancellationToken cancellationToken = default)
        {
            var pathJsonToken = await GetJTokenAsync(key, cancellationToken);

            if (pathJsonToken == default)
            {
                return default;
            }

            if (pathJsonToken.Type == JTokenType.Null)
            {
                return default;
            }

            if (pathJsonToken.Type == JTokenType.Integer)
            {
                return (int)pathJsonToken;
            }
            else if (pathJsonToken.Type == JTokenType.Float)
            {
                return (decimal)pathJsonToken;
            }
            else if (pathJsonToken.Type == JTokenType.String)
            {
                var pathValueString = pathJsonToken.ToObject<string>();
                if (decimal.TryParse(pathValueString, out var result))
                {
                    return result;
                }
                else
                {
                    throw new KeyValueException($"无法按数字读取json, key: {key}, 值{pathJsonToken}不是一个数字");
                }
            }
            else
            {
                throw new KeyValueException($"无法按数字读取json, key: {key}, 值{pathJsonToken}不是一个数字");
            }
        }

        public virtual async Task<bool?> GetBoolAsync(string key, CancellationToken cancellationToken = default)
        {
            var pathJsonToken = await GetJTokenAsync(key, cancellationToken);

            if (pathJsonToken == default)
            {
                return default;
            }

            if (pathJsonToken.Type == JTokenType.Null)
            {
                return default;
            }

            if (pathJsonToken.Type == JTokenType.Boolean)
            {
                return (bool)pathJsonToken;
            }
            throw new KeyValueException($"无法按布尔读取json, key: {key}, 值{pathJsonToken}不是一个布尔类型");
        }

        protected virtual async Task<JToken> GetJTokenAsync(string key, CancellationToken cancellationToken = default)
        {

            var (underlyingKey, jsonPath, subPath) = ExtractKeyPath(key);
            var valueString = await _keyValueStore.GetAsync(underlyingKey, cancellationToken);
            if (string.IsNullOrWhiteSpace(valueString))
            {
                valueString = "{}";
            }
            JToken rootJsonObj = default;
            var pathJsonToken = default(JToken);
            try
            {
                rootJsonObj = JToken.Parse(valueString);
            }
            catch (System.Exception e)
            {
                throw new KeyValueException($"无法读取json, key: {key}", e);
            }

            if (string.IsNullOrWhiteSpace(jsonPath))
            {
                pathJsonToken = rootJsonObj;
            }
            else
            {
                pathJsonToken = rootJsonObj[jsonPath];
            }

            if (!string.IsNullOrWhiteSpace(subPath))
            {
                if (pathJsonToken.Type == JTokenType.Object)
                {
                    var subPathObject = pathJsonToken as JObject;
                    if (subPathObject.ContainsKey(subPath))
                    {
                        pathJsonToken = subPathObject[subPath];
                    }
                }
                else
                {
                    pathJsonToken = pathJsonToken.SelectToken(subPath);
                }
            }
            return pathJsonToken;
        }

        protected (string key, string path, string subPath) ExtractKeyPath(string key)
        {
            var (type, city, company, path, subPath) = ExtractKey(key);
            switch (type)
            {
                case KeyType.Common:
                    return ("common." + path, default, subPath);
                case KeyType.City:
                    return ("city." + city, path, subPath);
                case KeyType.Company:
                    return ("company." + company, path, subPath);
            }
            throw new KeyValueException("不支持的key结构: ${key}");
        }

        protected (KeyType type, string city, string company, string key, string subPath) ExtractKey(string key)
        {
            key = key.Trim('/').Trim('.');
            var parts = key.Split('/');
            switch (parts.Length)
            {
                case 0:
                case 1:
                    return (KeyType.Common, default, default, key, default);
                case 2:
                    if (Guid.TryParse(parts[1], out _))
                    {
                        return (KeyType.Company, parts[0], parts[1], default, default);
                    }
                    return (KeyType.City, parts[0], default, parts[1], default);
                case 3:
                    if (Guid.TryParse(parts[1], out _))
                    {
                        return (KeyType.Company, parts[0], parts[1], parts[2], default);
                    }
                    return (KeyType.City, parts[0], default, parts[1], parts[2]);
                default:
                    throw new KeyValueException("key的层级超过3层");
            }
        }

        [Flags]
        protected enum KeyType
        {
            Common = 1,

            City = 2,

            Company = 4
        }
    }
}