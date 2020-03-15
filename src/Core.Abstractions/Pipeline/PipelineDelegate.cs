using System.Threading.Tasks;

namespace Core.Pipeline
{
    public delegate ValueTask PipelineDelegate<T>(T context);

    public delegate ValueTask PipelineNextDelegate<TSource, TContext>(TSource source, TContext context, PipelineDelegate<TContext> next);
}
