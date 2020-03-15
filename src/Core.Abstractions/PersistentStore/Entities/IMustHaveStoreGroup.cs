namespace Core.PersistentStore
{
    public interface IMustHaveStoreGroup : IMustHaveStoreGroupId
    {
        string GroupName { get; set; }
    }
}
