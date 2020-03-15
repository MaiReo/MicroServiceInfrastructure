using System;

namespace Core.PersistentStore
{
    /// <summary>
    /// Provides current DateTimeOffset.
    /// </summary>
    public interface IClockProvider
    {
        /// <summary>
        /// Get current DateTimeOffset.
        /// </summary>
        /// <returns></returns>
        DateTimeOffset GetDateTimeOffsetNow();
    }
}