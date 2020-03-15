using System;

namespace Core.PersistentStore
{
    public interface IMustHaveStoreGroupId
    {
        Guid GroupId { get; set; }
    }
}
