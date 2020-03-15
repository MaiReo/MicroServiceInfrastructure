namespace Core.PersistentStore
{
    public interface IMustHaveStore : IMustHaveStoreId
    {
        string StoreName { get; set; }
    }
}
