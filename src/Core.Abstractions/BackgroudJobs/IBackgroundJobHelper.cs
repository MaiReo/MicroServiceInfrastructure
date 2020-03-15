using System;

namespace Core.BackgroundJobs
{
    public interface IBackgroundJobHelper
    {
        string Enqueue<TJob, TArgs>(TArgs args, DateTimeOffset? enqueueAt = default) where TJob : IBackgroundJobWithArgs<TArgs> where TArgs : class, new();
    }
}
