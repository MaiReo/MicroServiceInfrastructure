using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues
{
    public interface IKeyValueReader
    {
        Task<T> GetObjectAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

        Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default);

        Task<decimal?> GetDecimalAsync(string key, CancellationToken cancellationToken = default);

        Task<bool?> GetBoolAsync(string key, CancellationToken cancellationToken = default);
    }
}