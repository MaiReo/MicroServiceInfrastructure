using System;
using System.Threading.Tasks;

namespace Core.Pipeline
{
    public interface IAsyncPipelineAggregator<T> : IAsyncPipelineAggregator
    {
        Task AggregateAsync(T context, Func<Task> next);
    }

    public interface IAsyncPipelineAggregator
    {
    }
}