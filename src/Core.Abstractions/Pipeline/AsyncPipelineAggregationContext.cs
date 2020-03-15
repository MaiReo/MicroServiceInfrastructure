using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Pipeline
{
    public abstract class AsyncPipelineAggregationContext
    {
        public static AsyncPipelineAggregationContext<T> Build<T>(IEnumerable<IAsyncPipelineContextHandler<T>> handlers, Func<Task> next)
        {
            return new AsyncPipelineAggregationContext<T>(handlers, next);
        }
    }

    public class AsyncPipelineAggregationContext<T> : AsyncPipelineAggregationContext
    {
        private readonly IEnumerator<IAsyncPipelineContextHandler<T>> _enumerator;
        private readonly Func<Task> _next;

        public AsyncPipelineAggregationContext(IEnumerable<IAsyncPipelineContextHandler<T>> handlers, Func<Task> next)
        {
            this._enumerator = handlers.OrderByDescending(x => x.Order).GetEnumerator();
            this._next = next;
        }

        public async Task AggregateAsync(T context)
        {
            if (!_enumerator.MoveNext())
            {
                await _next();
                return;
            }
            await _enumerator.Current.HandleAsync(context, async () => await AggregateAsync(context));
        }
    }
}