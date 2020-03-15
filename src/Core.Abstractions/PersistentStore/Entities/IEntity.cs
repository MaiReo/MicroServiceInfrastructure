namespace Core.PersistentStore
{
    /// <summary>
    /// 实体
    /// </summary>
    public interface IEntityBase
    {

    }

    /// <summary>
    /// 实体
    /// </summary>
    public interface IEntity<TKey> : IEntityBase
    {
        TKey Id { get; set; }
    }

    /// <summary>
    /// 实体
    /// </summary>
    public interface IEntity : IEntity<int>, IEntityBase
    {
    }
}
