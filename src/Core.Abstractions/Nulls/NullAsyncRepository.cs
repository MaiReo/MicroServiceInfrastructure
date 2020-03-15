using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.PersistentStore.Repositories
{
    public class NullAsyncRepository<TEntity, TKey> : IAsyncRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        public T Query<T>(Func<IQueryable<TEntity>, T> predicate)
        {
            return default;
        }

        public ValueTask<TEntity> FirstOrDefaultAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return new ValueTask<TEntity>(default(TEntity));
        }

        public ValueTask<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return new ValueTask<TEntity>(default(TEntity));
        }

        public ValueTask<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return new ValueTask<TEntity>(default(TEntity));
        }

        public ValueTask<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return new ValueTask<TEntity>(default(TEntity));
        }

        public ValueTask<TEntity> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return new ValueTask<TEntity>(default(TEntity));
        }

        public ValueTask<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return new ValueTask<TEntity>(default(TEntity));
        }

        public ValueTask EnsureCollectionLoadedAsync<T>(TEntity entity, Expression<Func<TEntity, IEnumerable<T>>> collectionSelector, CancellationToken cancellationToken = default) where T : class
        {
            return new ValueTask();
        }

        public ValueTask EnsureNavigationLoadedAsync<T>(TEntity entity, Expression<Func<TEntity, T>> propertySelector, CancellationToken cancellationToken = default) where T : class
        {
            return new ValueTask();
        }

        public IQueryable<TEntity> GetAll()
        {
            return Enumerable.Empty<TEntity>().AsQueryable();
        }
    }
}
