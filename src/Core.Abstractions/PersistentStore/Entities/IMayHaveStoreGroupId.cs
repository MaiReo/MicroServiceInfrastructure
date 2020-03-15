using System;

namespace Core.PersistentStore
{
    public interface IMayHaveStoreGroupId
    {
        Guid? GroupId { get; set; }
    }

}
