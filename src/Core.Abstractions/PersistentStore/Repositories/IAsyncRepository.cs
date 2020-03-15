using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;


namespace Core.PersistentStore.Repositories
{
    public interface IAsyncRepository<TEntity> : IAsyncRepository<TEntity, int>, IRepository<TEntity, int>, IRepository<TEntity> where TEntity : class, IEntity
    {
    }

    public interface IAsyncRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        ValueTask<TEntity> FirstOrDefaultAsync(TKey id, CancellationToken cancellationToken = default);

        ValueTask<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        ValueTask<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        ValueTask<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        ValueTask<TEntity> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        ValueTask<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        ValueTask EnsureNavigationLoadedAsync<T>(TEntity entity, Expression<Func<TEntity, T>> propertySelector, CancellationToken cancellationToken = default) where T : class;

        ValueTask EnsureCollectionLoadedAsync<T>(TEntity entity, Expression<Func<TEntity, IEnumerable<T>>> collectionSelector, CancellationToken cancellationToken = default) where T : class;
    }
}
