using System;
using System.Threading.Tasks;

namespace Core.Pipeline
{
    public interface IAsyncPipelineContextHandler
    {
        int Order { get; }
    }

    public interface IAsyncPipelineContextHandler<T> : IAsyncPipelineContextHandler
    {
        Task HandleAsync(T context, Func<Task> next);
    }
}