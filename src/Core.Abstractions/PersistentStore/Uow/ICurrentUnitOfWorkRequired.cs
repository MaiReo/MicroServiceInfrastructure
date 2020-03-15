namespace Core.PersistentStore.Uow
{
    public interface ICurrentUnitOfWorkRequired
    {
        ICurrentUnitOfWork CurrentUnitOfWork { get; set; }
    }
}
