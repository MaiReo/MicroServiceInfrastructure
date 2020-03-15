using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.PersistentStore.Repositories
{
    public abstract class EFAsyncRepository<TDbContext, TEntity> : EFAsyncRepository<TDbContext, TEntity, int>, IAsyncRepository<TEntity>
        where TDbContext : DbContext
        where TEntity : class, IEntity
    {
        protected EFAsyncRepository(IDbContextResolver<TDbContext> dbContextResolver) : base(dbContextResolver)
        {
        }
    }

    public abstract class EFAsyncRepository<TDbContext, TEntity, TKey> : IRepositoryWithDbContext, IAsyncRepository<TEntity, TKey>
        where TDbContext : DbContext
        where TEntity : class, IEntity<TKey>
    {
        private readonly IDbContextResolver<TDbContext> _dbContextResolver;

        protected EFAsyncRepository(IDbContextResolver<TDbContext> dbContextResolver)
        {
            this._dbContextResolver = dbContextResolver;
        }

        protected virtual DbContext DbContext => _dbContextResolver.GetDbContext();

        public virtual async ValueTask<TEntity> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var dbContext = DbContext;
            var entity = await dbContext.FindAsync<TEntity>(new object[] { id }, cancellationToken);
            if (entity == default)
            {
                return default;
            }
            dbContext.Entry(entity).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async ValueTask<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var dbContext = DbContext;
            dbContext.Entry(entity).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async ValueTask<TEntity> FirstOrDefaultAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var dbContext = DbContext;
            var entity = await dbContext.FindAsync<TEntity>(new object[] { id }, cancellationToken);
            return entity;
        }

        public virtual async ValueTask<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var dbContext = DbContext;
            var entity = await dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
            return entity;
        }

        public virtual async ValueTask<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var dbContext = DbContext;
            var entry = dbContext.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }

        public virtual async ValueTask EnsureNavigationLoadedAsync<T>(TEntity entity, Expression<Func<TEntity, T>> propertySelector, CancellationToken cancellationToken = default) where T : class
        {
            var propertyName = (propertySelector.Body as MemberExpression)?.Member?.Name;
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }
            var dbContext = DbContext;
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                dbContext.Attach(entity);
            }
            var nav = entry.Navigation(propertyName);
            if (nav.IsLoaded)
            {
                return;
            }
            await nav.LoadAsync(cancellationToken);
        }

        public virtual async ValueTask EnsureCollectionLoadedAsync<T>(TEntity entity, Expression<Func<TEntity, IEnumerable<T>>> collectionSelector, CancellationToken cancellationToken = default) where T : class
        {
            var dbContext = DbContext;
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                dbContext.Attach(entity);
            }
            var collection = entry.Collection(collectionSelector); 
            if (collection.IsLoaded)
            {
                return;
            }
            await collection.LoadAsync(cancellationToken);
        }

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return predicate(DbContext.Set<TEntity>());
        }

        public virtual async ValueTask<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var dbContext = DbContext;
            var entry = dbContext.Entry(entity);
            if (entry.State != EntityState.Modified)
            {
                entry.State = EntityState.Modified;
            }
            await dbContext.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbContext.Set<TEntity>();
        }

        public DbContext GetDbContext() => DbContext;

    }
}
