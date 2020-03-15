using Microsoft.EntityFrameworkCore;

namespace Core.PersistentStore.Repositories.Extensions
{
    public static class RepositoryExtensions
    {
        public static DbContext GetDbContext<TEntity, TKey>(this IRepository<TEntity, TKey> repository)
            where TEntity : class, IEntity<TKey>
        {
            if (!(repository is IRepositoryWithDbContext repositoryWithDbContext))
            {
                return default;
            }
            var dbContext = repositoryWithDbContext.GetDbContext();
            return dbContext;
        }
    }
}
