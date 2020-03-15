using Core.PersistentStore;
using Core.PersistentStore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.PersistentStore.Repositories
{
    public abstract class AsyncCommonRepositoryBase<TEntity> : AsyncCommonRepositoryBase<TEntity, int>, IRepository<TEntity, int> where TEntity : class, IEntity
    {
    }

    public abstract class AsyncCommonRepositoryBase<TEntity, TKey> : IAsyncRepository<TEntity, TKey>, IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        public abstract ValueTask<TEntity> DeleteAsync(TKey id, CancellationToken cancellationToken = default);

        public virtual ValueTask<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return DeleteAsync(entity.Id, cancellationToken);
        }
        

        public virtual ValueTask EnsureCollectionLoadedAsync<T>(TEntity entity, Expression<Func<TEntity, IEnumerable<T>>> collectionSelector, CancellationToken cancellationToken = default) where T : class
        {
            return new ValueTask();
        }

        public virtual ValueTask EnsureNavigationLoadedAsync<T>(TEntity entity, Expression<Func<TEntity, T>> propertySelector, CancellationToken cancellationToken = default) where T : class
        {
            return new ValueTask();
        }

        public virtual ValueTask<TEntity> FirstOrDefaultAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return FirstOrDefaultAsync(CreateEqualityExpressionForId(id), cancellationToken);
        }

        public virtual ValueTask<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return new ValueTask<TEntity>(GetAll().FirstOrDefault(predicate));
        }

        public abstract IQueryable<TEntity> GetAll();

        public abstract ValueTask<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> predicate)
        {
            if (predicate is null)
            {
                return default;
            }
            return predicate(GetAll());
        }

        public abstract ValueTask<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);


        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TKey id)
        {
            var idMember = typeof(IEntity<TKey>).GetMember(nameof(IEntity<TKey>.Id))[0];
            var parameter = Expression.Parameter(typeof(TEntity));
            var body = Expression.Equal(Expression.MakeMemberAccess(parameter, idMember), Expression.Constant(id, typeof(TKey)));
            Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
            return lambda;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateDefaultValueEqualityExpressionForId()
        {
            var idMember = typeof(IEntity<TKey>).GetMember(nameof(IEntity<TKey>.Id))[0];
            var parameter = Expression.Parameter(typeof(TEntity));
            var body = Expression.Equal(Expression.MakeMemberAccess(parameter, idMember), Expression.Default(typeof(TKey)));
            Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);
            return lambda;
        }
    }
}
