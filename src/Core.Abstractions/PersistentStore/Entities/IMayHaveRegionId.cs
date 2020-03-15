using System;

namespace Core.PersistentStore
{
    public interface IMayHaveRegionId
    {
        Guid? RegionId { get; set; }
    }
}
