using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues
{
    public class NullKeyValueWriter : NullKeyValueReader, IKeyValueWriter
    {
        public Task WriteAsync<T>(string key, T value, bool forceOverride = false, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public static new NullKeyValueWriter Instance => new NullKeyValueWriter();
    }
}