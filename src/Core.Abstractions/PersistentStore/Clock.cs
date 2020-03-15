using System;

namespace Core.PersistentStore
{
    /// <summary>
    /// Wrapper of DatetimeOffset.Now
    /// </summary>
    public class Clock
    {

        static Clock()
        {
            ClockProvider = CLRTypeDateTimeOffsetClockProvider.Instance;
        }

        /// <summary>
        /// Provides current DateTimeOffset.
        /// </summary>
        public static IClockProvider ClockProvider { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static DateTimeOffset Now  => ClockProvider.GetDateTimeOffsetNow();
    }
}
