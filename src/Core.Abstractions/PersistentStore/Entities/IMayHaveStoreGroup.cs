namespace Core.PersistentStore
{

    public interface IMayHaveStoreGroup : IMayHaveStoreGroupId
    {
        string GroupName { get; set; }
    }

}
