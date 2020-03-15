using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.PersistentStore
{
    /// <summary>
    /// See <see cref="IClockProvider"/>
    /// </summary>
    public class CLRTypeDateTimeOffsetClockProvider : IClockProvider
    {
        /// <summary>
        /// Gets a new instance of <see cref="CLRTypeDateTimeOffsetClockProvider"/> 
        /// </summary>
        public static CLRTypeDateTimeOffsetClockProvider Instance => new CLRTypeDateTimeOffsetClockProvider();

        /// <summary>
        /// See <see cref="IClockProvider.GetDateTimeOffsetNow"/>
        /// </summary>
        /// <returns></returns>
        public virtual DateTimeOffset GetDateTimeOffsetNow() => DateTimeOffset.Now;
    }
}
