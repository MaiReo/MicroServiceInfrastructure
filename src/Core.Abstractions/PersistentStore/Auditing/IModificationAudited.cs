namespace Core.PersistentStore.Auditing
{
    public interface IModificationAudited : IHasModificationTime
    {
        string LastModifierUserId { get; set; }

        string LastModifierUserName { get; set; }
    }
}
