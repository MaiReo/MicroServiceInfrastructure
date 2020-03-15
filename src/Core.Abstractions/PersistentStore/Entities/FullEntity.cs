using System;
using System.Collections.Generic;
using System.Text;

namespace Core.PersistentStore
{

    public abstract class FullEntity : FullEntity<int>, IHasCreationTime, IHasModificationTime, IHasDeletionTime, ISoftDelete, IEntity<int>, IEntity, IEntityBase
    {
    }

    public abstract class FullEntity<T> : Entity<T>, IHasCreationTime, IHasModificationTime, IHasDeletionTime, ISoftDelete, IEntity<T>, IEntityBase
    {
        public virtual DateTimeOffset CreationTime { get; set; }

        public virtual DateTimeOffset? LastModificationTime { get; set; }

        public bool IsDeleted { get; set; }

        public virtual DateTimeOffset? DeletionTime { get; set; }
    }
}
