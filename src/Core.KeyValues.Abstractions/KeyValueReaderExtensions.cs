using System.Threading;
using System.Threading.Tasks;

namespace Core.KeyValues
{
    public static class KeyValueReaderExtensions
    {
        public static async Task<int?> GetInt32Async(this IKeyValueReader @this, string key, CancellationToken cancellationToken = default)
        {
            var decimalValue = await @this.GetDecimalAsync(key, cancellationToken);
            return (int?)decimalValue;
        }

        public static async Task<double?> GetDoubleAsync(this IKeyValueReader @this, string key, CancellationToken cancellationToken = default)
        {
            var decimalValue = await @this.GetDecimalAsync(key, cancellationToken);
            return (double?)decimalValue;
        }

        public static double? GetDouble(this IKeyValueReader @this, string key)
        {
            var task = Task.Run(async () => await GetDoubleAsync(@this, key).ConfigureAwait(false));
            Task.WaitAll(task);
            return task.Result;
        }

        public static int? GetInt32(this IKeyValueReader @this, string key)
        {
            var task = Task.Run(async () => await GetInt32Async(@this, key).ConfigureAwait(false));
            Task.WaitAll(task);
            return task.Result;
        }

        public static bool? GetBool(this IKeyValueReader @this, string key)
        {
            var task = Task.Run(async () => await @this.GetBoolAsync(key).ConfigureAwait(false));
            Task.WaitAll(task);
            return task.Result;
        }

        public static T GetObject<T>(this IKeyValueReader @this, string key) where T : class, new()
        {
            var task = Task.Run(async () => await @this.GetObjectAsync<T>(key).ConfigureAwait(false));
            Task.WaitAll(task);
            return task.Result;
        }

        public static string GetString(this IKeyValueReader @this, string key)
        {
            var task = Task.Run(async () => await @this.GetStringAsync(key).ConfigureAwait(false));
            Task.WaitAll(task);
            return task.Result;
        }
    }
}