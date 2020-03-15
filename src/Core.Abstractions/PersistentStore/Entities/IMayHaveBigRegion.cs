namespace Core.PersistentStore
{
    public interface IMayHaveBigRegion : IMayHaveBigRegionId
    {
        string BigRegionName { get; set; }
    }

}
