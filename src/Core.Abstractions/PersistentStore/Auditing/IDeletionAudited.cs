namespace Core.PersistentStore.Auditing
{
    public interface IDeletionAudited : IHasDeletionTime, ISoftDelete
    {
        string DeleterUserId { get; set; }

        string DeleterUserName { get; set; }
    }

}
