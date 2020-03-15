using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues.Json
{
    public class KeyPassThroughValueWrapedAsJsonObjectKeyValueReaderWriter : KeyPassThroughValueWrapedAsJsonObjectKeyValueReader, IKeyValueWriter
    {
        private readonly IKeyValueStore _store;

        public KeyPassThroughValueWrapedAsJsonObjectKeyValueReaderWriter(IKeyValueStore store) : base(store)
        {
            _store = store;
        }

        public async Task WriteAsync<T>(string key, T value, bool forceOverride = false, CancellationToken cancellationToken = default)
        {
            var wrappedObject = new Dictionary<string, T>
            {
                { PLACEHOLDER_PROPERTY_NAME, value }
            };
            var json = JsonConvert.SerializeObject(wrappedObject);
            await _store.SetAsync(key, json, cancellationToken);
        }
    }
}
