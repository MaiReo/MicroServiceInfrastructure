using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.PersistentStore
{

    /// <summary>
    /// 实体
    /// </summary>
    /// <typeparam name="T">主键类型</typeparam>
    public abstract class Entity : Entity<int>, IEntity<int>, IEntity, IEntityBase
    {
    }

    /// <summary>
    /// 实体
    /// </summary>
    /// <typeparam name="T">主键类型</typeparam>
    public abstract class Entity<T> : IEntity<T>, IEntityBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual T Id { get; set; }
    }
}
