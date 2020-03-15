namespace Core.PersistentStore
{
    public interface IMayHaveRegion : IMayHaveRegionId
    {
        string RegionName { get; set; }
    }
}
