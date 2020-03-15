using Core.PersistentStore;
using Core.PersistentStore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Web.EFCore
{
    public class WebDbContext : CorePersistentStoreDbContext
    {
        public virtual DbSet<WebEntity> WebEntities { get; set; }
        public WebDbContext(DbContextOptions<WebDbContext> options) : base(options)
        {
        }
    }


    public class WebRepository<TEntity> : EFAsyncRepository<WebDbContext, TEntity>, IAsyncRepository<TEntity>, IRepository<TEntity>
        where TEntity : class, IEntity
    {
        public WebRepository(IDbContextResolver<WebDbContext> dbContextResolver) : base(dbContextResolver)
        {
        }
    }

    public class WebRepository<TEntity, TKey> : EFAsyncRepository<WebDbContext, TEntity, TKey>, IAsyncRepository<TEntity, TKey>, IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        public WebRepository(IDbContextResolver<WebDbContext> dbContextResolver) : base(dbContextResolver)
        {
        }
    }

    public class WebEntity : FullEntity, IEntity
    {
        public string Name { get; set; }
    }
}
