using System;

namespace Core.PersistentStore
{
    public interface IMayHaveBigRegionId
    {
        Guid? BigRegionId { get; set; }
    }

}
