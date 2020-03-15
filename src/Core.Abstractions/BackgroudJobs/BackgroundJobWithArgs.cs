using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace Core.BackgroundJobs
{
    public abstract class BackgroundJobWithArgs<TArgs> : IBackgroundJobWithArgs<TArgs> where TArgs : class, new()
    {
        public BackgroundJobWithArgs()
        {
        }

        public abstract Task ExecuteAsync(TArgs args);
    }
}
