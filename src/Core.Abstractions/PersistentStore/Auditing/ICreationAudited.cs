namespace Core.PersistentStore.Auditing
{
    public interface ICreationAudited : IHasCreationTime
    {
        string CreationUserId { get; set; }

        string CreationUserName { get; set; }
    }
}
