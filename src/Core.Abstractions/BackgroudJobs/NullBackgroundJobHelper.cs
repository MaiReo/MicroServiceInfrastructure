using System;

namespace Core.BackgroundJobs
{
    public class NullBackgroundJobHelper : IBackgroundJobHelper
    {
        public static NullBackgroundJobHelper Instance => new NullBackgroundJobHelper();

        public string Enqueue<TJob, TArgs>(TArgs args, DateTimeOffset? enqueueAt = null)
            where TJob : IBackgroundJobWithArgs<TArgs>
            where TArgs : class, new()
        {
            return string.Empty;
        }
    }
}
