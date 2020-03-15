namespace Core.Session
{
    internal class CoreSessionContainer<T> : ICoreSessionContainer<T, string>, ICoreSessionContainer<T>
    {
        public CoreSessionContainer()
        {
        }

        public CoreSessionContainer(T id) => Id = id;

        public CoreSessionContainer(T id, string name) : this(id) => Name = name;

        public virtual T Id { get; protected set; }

        public virtual string Name { get; protected set; }
    }
    public static class CoreSessionContainer
    {
        public static ICoreSessionContainer<T> Create<T>(T id) => new CoreSessionContainer<T>(id);
        public static ICoreSessionContainer<T, string> Create<T>(T id, string name) => new CoreSessionContainer<T>(id, name);
    }
}