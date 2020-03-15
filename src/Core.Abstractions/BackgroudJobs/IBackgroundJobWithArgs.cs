using System.Threading.Tasks;

namespace Core.BackgroundJobs
{
    public interface IBackgroundJobWithArgs<TArgs> where TArgs : class, new()
    {
        Task ExecuteAsync(TArgs args);
    }
}
