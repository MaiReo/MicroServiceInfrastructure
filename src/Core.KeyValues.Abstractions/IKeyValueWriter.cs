using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues
{
    public interface IKeyValueWriter : IKeyValueReader
    {
        Task WriteAsync<T>(string key, T value, bool forceOverride = false, CancellationToken cancellationToken = default);
    }
}