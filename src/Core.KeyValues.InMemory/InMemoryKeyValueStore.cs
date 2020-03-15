using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues
{
    public class InMemoryKeyValueStore : IKeyValueStore
    {
        public readonly Dictionary<string, string> _caches;

        public InMemoryKeyValueStore()
        {
            _caches = new Dictionary<string, string>();
        }

        public Task CopyAsync(string oldKeyPrefix, string newKeyPrefix, CancellationToken cancellationToken = default)
        {
            var oldKeys = _caches.Keys.Where(x => x.StartsWith(oldKeyPrefix)).ToList();
            foreach (var oldKey in oldKeys)
            {
                var value = _caches[oldKey];
                var newKey = newKeyPrefix;
                if (oldKey.Length > oldKeyPrefix.Length)
                {
                    newKey +=oldKey.Substring(oldKeyPrefix.Length);
                }
                _caches.Add(newKey, value);
            }
            return Task.CompletedTask;
        }

        public Task<string> GetAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            var result = (_caches.TryGetValue(key, out var value)) ? value : default;
            return Task.FromResult(result);
        }

        public Task SetAsync(
            string key,
            string value,
            CancellationToken cancellationToken = default)
        {
            if (_caches.ContainsKey(key))
            {
                _caches[key] = value;
            }
            else
            {
                _caches.Add(key, value);
            }
            return Task.CompletedTask;
        }
    }
}