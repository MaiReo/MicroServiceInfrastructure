namespace Core.Session
{
    public interface ICoreSessionContainer<T>
    {
        T Id { get; }
    }

    public interface ICoreSessionContainer<TId, TName> : ICoreSessionContainer<TId>
    {
        TName Name { get; }
    }
}