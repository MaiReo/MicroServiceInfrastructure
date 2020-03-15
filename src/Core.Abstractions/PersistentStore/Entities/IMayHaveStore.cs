using System;

namespace Core.PersistentStore
{
    public interface IMayHaveStore : IMayHaveStoreId
    {
        string StoreName { get; set; }
    }
}
