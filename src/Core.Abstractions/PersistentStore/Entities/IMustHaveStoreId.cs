using System;

namespace Core.PersistentStore
{
    public interface IMustHaveStoreId
    {
        Guid StoreId { get; set; }
    }
}
