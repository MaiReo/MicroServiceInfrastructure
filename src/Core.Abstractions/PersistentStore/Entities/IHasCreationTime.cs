using System;

namespace Core.PersistentStore
{
    /// <summary>
    /// 有创建时间
    /// </summary>
    public interface IHasCreationTime
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTimeOffset CreationTime { get; set; }
    }
}
