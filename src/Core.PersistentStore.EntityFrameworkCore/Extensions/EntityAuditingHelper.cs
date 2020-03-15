using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.PersistentStore.Auditing.Extensions
{
    public static class EntityAuditingExtensions
    {
        public static void PerformAuditing(this EntityEntry entry, string currentUserId, string currentUserName)
        {
            if (entry == default)
            {
                return;
            }
            PerformAddAuditing(entry, currentUserId, currentUserName);
            PerformModificationAuditing(entry, currentUserId, currentUserName);
            PerformDeletionAuditing(entry, currentUserId, currentUserName);
        }

        private static void PerformAddAuditing(EntityEntry entry, string currentUserId, string currentUserName)
        {
            if (entry.State != EntityState.Added)
            {
                return;
            }
            if (!(entry.Entity is IEntityBase))
            {
                return;
            }
            if (entry.Entity is IHasCreationTime hasCreationTime)
            {
                hasCreationTime.CreationTime = Clock.Now;
            }

            if (entry.Entity is IHasModificationTime hasModificationTime)
            {
                hasModificationTime.LastModificationTime = null;
            }

            if (entry.Entity is ICreationAudited audited)
            {
                if (string.IsNullOrWhiteSpace(audited.CreationUserId))
                {
                    audited.CreationUserId = currentUserId;
                }
                if (string.IsNullOrWhiteSpace(audited.CreationUserName))
                {
                    audited.CreationUserName = currentUserName;
                }
            }
        }

        private static void PerformModificationAuditing(EntityEntry entry, string currentUserId, string currentUserName)
        {
            if (entry.State != EntityState.Modified)
            {
                return;
            }
            if (!(entry.Entity is IEntityBase))
            {
                return;
            }
            if (entry.Entity is IHasModificationTime hasModificationTime)
            {
                hasModificationTime.LastModificationTime = Clock.Now;
            }

            if (entry.Entity is IModificationAudited audited)
            {
                if (string.IsNullOrWhiteSpace(audited.LastModifierUserId))
                {
                    audited.LastModifierUserId = currentUserId;
                }
                if (string.IsNullOrWhiteSpace(audited.LastModifierUserName))
                {
                    audited.LastModifierUserName = currentUserName;
                }
            }
            if (entry.Entity is ISoftDelete softDelete && softDelete.IsDeleted)
            {
                if (entry.Entity is IHasDeletionTime hasDeletionTime)
                {
                    hasDeletionTime.DeletionTime = Clock.Now;
                }
                if (entry.Entity is IDeletionAudited deletionAudited)
                {
                    if (string.IsNullOrWhiteSpace(deletionAudited.DeleterUserId))
                    {
                        deletionAudited.DeleterUserId = currentUserId;
                    }
                    if (string.IsNullOrWhiteSpace(deletionAudited.DeleterUserName))
                    {
                        deletionAudited.DeleterUserName = currentUserName;
                    }
                }
            }
        }

        private static void PerformDeletionAuditing(EntityEntry entry, string currentUserId, string currentUserName)
        {
            if (entry.State != EntityState.Deleted)
            {
                return;
            }
            if (!(entry.Entity is IEntityBase))
            {
                return;
            }
            if (entry.Entity is IEntityBase && entry.Entity is ISoftDelete)
            {
                entry.Reload();
                if (entry.State == EntityState.Unchanged)
                {
                    entry.State = EntityState.Modified;
                    (entry.Entity as ISoftDelete).IsDeleted = true;
                    if (entry.Entity is IHasDeletionTime hasDeletionTime)
                    {
                        hasDeletionTime.DeletionTime = Clock.Now;
                    }
                    if (entry.Entity is IDeletionAudited deletionAudited)
                    {
                        if (string.IsNullOrWhiteSpace(deletionAudited.DeleterUserId))
                        {
                            deletionAudited.DeleterUserId = currentUserId;
                        }
                        if (string.IsNullOrWhiteSpace(deletionAudited.DeleterUserName))
                        {
                            deletionAudited.DeleterUserName = currentUserName;
                        }
                    }
                }
            }
        }
    }
}
