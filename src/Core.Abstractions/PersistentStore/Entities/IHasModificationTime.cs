using System;

namespace Core.PersistentStore
{
    /// <summary>
    /// 有上次修改时间
    /// </summary>
    public interface IHasModificationTime
    {
        /// <summary>
        /// 上次修改时间
        /// </summary>
        DateTimeOffset? LastModificationTime { get; set; }
    }
}
