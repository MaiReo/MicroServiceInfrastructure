using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Pipeline
{
    public class AsyncPipelineAggregator<T> : IAsyncPipelineAggregator<T>
    {
        private readonly IAsyncPipelineContextHandler<T>[] handlers;

        public AsyncPipelineAggregator(IEnumerable<IAsyncPipelineContextHandler<T>> handlers)
        {
            this.handlers = handlers.ToArray();
        }

        public async Task AggregateAsync(T context, Func<Task> next)
        {
            var aggregationContext = AsyncPipelineAggregationContext.Build(handlers, next);
            await aggregationContext.AggregateAsync(context);
        }
    }
}