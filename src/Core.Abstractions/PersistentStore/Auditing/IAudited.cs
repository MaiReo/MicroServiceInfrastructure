namespace Core.PersistentStore.Auditing
{

    public interface IAudited : ICreationAudited, IHasCreationTime, IModificationAudited, IHasModificationTime
    {

    }

}
