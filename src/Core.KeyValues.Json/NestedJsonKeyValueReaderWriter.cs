using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues.Json
{
    [Obsolete("Use KeyPassThroughValueWrapedAsJsonObjectKeyValueReaderWriter instead.")]
    public class NestedJsonKeyValueReaderWriter : NestedJsonKeyValueReader, IKeyValueReader, IKeyValueWriter
    {
        private readonly IKeyValueStore _keyValueStore;
        public NestedJsonKeyValueReaderWriter(IKeyValueStore keyValueStore) : base(keyValueStore)
        {
            _keyValueStore = keyValueStore;
        }

        public async Task WriteAsync<T>(string key, T value, bool forceOverride = false, CancellationToken cancellationToken = default)
        {
            var (keyType, _, _, _, _) = ExtractKey(key);
            var (underlyingKey, jsonPath, subPath) = ExtractKeyPath(key);
            var isFullOverride = false;
            switch (keyType)
            {
                case KeyType.Common:
                    isFullOverride = true;
                    break;
                case KeyType.City:
                case KeyType.Company:
                    isFullOverride = string.IsNullOrWhiteSpace(jsonPath);
                    break;
            }
            var rootValueString = "{}";
            try
            {
                rootValueString = await _keyValueStore.GetAsync(underlyingKey, cancellationToken);
            }
            catch (KeyNotExistException)
            {
                if (!isFullOverride)
                {
                    //TODO 直接覆盖
                    if (!forceOverride)
                    {
                        throw;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(rootValueString))
            {
                rootValueString = "{}";
            }

            JToken rootValue = default;
            try
            {
                rootValue = JToken.Parse(rootValueString);
            }
            catch (System.Exception e)
            {
                if (!isFullOverride)
                {
                    throw new KeyValueException($"无法读取json, key: {key}", e);
                }
            }

            var newValue = JToken.FromObject(value);

            if (isFullOverride)
            {
                rootValue = newValue;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(subPath))
                {
                    rootValue[jsonPath] = newValue;
                }
                else
                {
                    var subToken = rootValue[jsonPath];
                    if (subToken != null)
                    {
                        rootValue[jsonPath][subPath] = newValue;
                    }
                    else
                    {
                        rootValue[jsonPath] = JToken.FromObject(new Dictionary<string, object>
                        {
                            { subPath, newValue },
                        });
                    }
                }
            }
            var sb = new System.Text.StringBuilder();
            using var sWriter = new System.IO.StringWriter(sb);
            using var jWriter = new JsonTextWriter(sWriter);
            await rootValue.WriteToAsync(jWriter);
            var newValueString = sb.ToString();
            await _keyValueStore.SetAsync(underlyingKey, newValueString, cancellationToken);
        }
    }
}