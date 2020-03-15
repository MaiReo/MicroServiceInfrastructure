using System;

namespace Core.PersistentStore
{
    public interface IMayHaveStoreId
    {
        Guid? StoreId { get; set; }
    }
}
