using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues
{
    public class NullKeyValueReader : IKeyValueReader
    {
        public Task<T> GetObjectAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            return Task.FromResult<T>(default);
        }

        public Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(string));
        }

        public Task<decimal?> GetDecimalAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(decimal?));
        }

        public Task<bool?> GetBoolAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(bool?));
        }

        public static NullKeyValueReader Instance => new NullKeyValueReader();
    }
}