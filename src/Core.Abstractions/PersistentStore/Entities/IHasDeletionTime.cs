using System;

namespace Core.PersistentStore
{
    public interface IHasDeletionTime : ISoftDelete
    {
        DateTimeOffset? DeletionTime { get; set; }
    }

}
