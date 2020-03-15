using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Pipeline
{
    public static class PipelineExtensions
    {

        public static async ValueTask PipingAsync<TSource, TContext>(
            this IEnumerable<TSource> source, TContext context,
            PipelineNextDelegate<TSource,TContext> next,
            CancellationToken cancellationToken = default)
        {
            if (source is null)
            {
                return;
            }
            using (var enumlator = source.GetEnumerator())
            {
                async ValueTask  pipingAsync(TContext cxt)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    if (enumlator.MoveNext())
                    {
                        await next(enumlator.Current, cxt, pipingAsync);
                    }
                }
                await pipingAsync(context);
            }
        }
    }
}
