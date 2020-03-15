using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues.Json
{
    public class KeyPassThroughValueWrapedAsJsonObjectKeyValueReader : IKeyValueReader
    {
        public const string PLACEHOLDER_PROPERTY_NAME = "value";
        private readonly IKeyValueStore _store;

        public KeyPassThroughValueWrapedAsJsonObjectKeyValueReader(
            IKeyValueStore store)
        {
            _store = store;
        }

        public async Task<bool?> GetBoolAsync(string key, CancellationToken cancellationToken = default)
        {
            var valueToken = await GetValueAsync(key, cancellationToken);

            if (valueToken == default)
            {
                return default;
            }

            if (valueToken.Type == JTokenType.Null)
            {
                return default;
            }

            if (valueToken.Type == JTokenType.Boolean)
            {
                return (bool)valueToken;
            }
            throw new KeyValueException($"无法按布尔读取json, key: {key}, 值{valueToken}不是一个布尔类型");
        }

        public async Task<decimal?> GetDecimalAsync(string key, CancellationToken cancellationToken = default)
        {
            var valueToken = await GetValueAsync(key, cancellationToken);

            if (valueToken == default)
            {
                return default;
            }

            if (valueToken.Type == JTokenType.Null)
            {
                return default;
            }

            if (valueToken.Type == JTokenType.Integer)
            {
                return (int)valueToken;
            }
            else if (valueToken.Type == JTokenType.Float)
            {
                return (decimal)valueToken;
            }
            else if (valueToken.Type == JTokenType.String)
            {
                var pathValueString = valueToken.ToObject<string>();
                if (decimal.TryParse(pathValueString, out var result))
                {
                    return result;
                }
                else
                {
                    throw new KeyValueException($"无法按数字读取json, key: {key}, 值{valueToken}不是一个数字");
                }
            }
            else
            {
                throw new KeyValueException($"无法按数字读取json, key: {key}, 值{valueToken}不是一个数字");
            }

        }

        public async Task<T> GetObjectAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var valueToken = await GetValueAsync(key, cancellationToken);
            if (valueToken == default)
            {
                return default;
            }

            if (valueToken.Type == JTokenType.Null)
            {
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                if (valueToken.Type == JTokenType.String)
                {
                    return (T)(object)valueToken.ToObject<string>();
                }
                else if (valueToken.Type != JTokenType.Object)
                {
                    return (T)(object)valueToken.ToString();
                }
            }
            var obj = valueToken.ToObject<T>();
            return obj;
        }

        public virtual async Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            var valueToken = await GetValueAsync(key, cancellationToken);

            if (valueToken == default)
            {
                return default;
            }

            if (valueToken.Type == JTokenType.Null)
            {
                return default;
            }

            if (valueToken.Type == JTokenType.String)
            {
                return valueToken.Value<string>();
            }
            else
            {
                return valueToken.ToString();
            }
        }

        protected virtual async Task<JToken> GetValueAsync(string key, CancellationToken cancellationToken = default)
        {
            var valueString = await _store.GetAsync(key, cancellationToken);
            if (string.IsNullOrWhiteSpace(valueString))
            {
                return default;
            }
            JObject jObj;
            try
            {
                jObj = JObject.Parse(valueString);
            }
            catch (JsonReaderException e)
            {
                throw new KeyValueException($"无法按Json的Object读取key的包装: {key}", e);
            }
            if (jObj is null)
            {
                throw new KeyValueException($"按Json的Object读取key的包装出错: {key}");
            }
            if (!jObj.ContainsKey(PLACEHOLDER_PROPERTY_NAME))
            {
                throw new KeyValueException($"无法按Json的Object读取key的包装: {key}", $"缺少占位符属性{PLACEHOLDER_PROPERTY_NAME}");
            }
            var valueToken = jObj[PLACEHOLDER_PROPERTY_NAME];

            return valueToken;
        }
    }
}
