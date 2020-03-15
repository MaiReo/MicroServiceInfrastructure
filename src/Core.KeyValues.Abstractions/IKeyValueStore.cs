using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues
{
    public interface IKeyValueStore
    {
        Task<string> GetAsync(string key, CancellationToken cancellationToken = default);

        Task SetAsync(string key, string value, CancellationToken cancellationToken = default);

        Task CopyAsync(string oldKeyPrefix, string newKeyPrefix, CancellationToken cancellationToken = default);
    }
}