using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.PersistentStore.Repositories
{
    public static class ElasticSearchRepositoryExtensions
    {
        public static ValueTask<bool> CreateAsync<T>(
            this IElasticSearchRepository<T> @this,
            string index, Guid id, T item, CancellationToken cancellationToken = default) where T : class, new()
        {
            return @this.CreateAsync(index, id.ToString(), item, cancellationToken);
        }

        public static ValueTask<Exception> CreateAsync<T>(
            this IElasticSearchRepository<T> @this,
            string index,
            Func<T, Guid> idSelector,
            IEnumerable<T> documents,
            bool replace = false,
            CancellationToken cancellationToken = default) where T : class, new()
        {
            return @this.CreateAsync(
                index,
                (T model) => idSelector?.Invoke(model).ToString(),
                documents,
                replace,
                cancellationToken);
        }

        public static ValueTask<bool> UpdateAsync<T>(
            this IElasticSearchRepository<T> @this,
            string index,
            Guid id,
            T item,
            CancellationToken cancellationToken = default) where T : class, new()
        {
            return @this.UpdateAsync(index, id.ToString(), item, cancellationToken);
        }

        public static ValueTask<bool> UpdateAsync<T>(
            this IElasticSearchRepository<T> @this,
            string index, Guid id,
            CancellationToken cancellationToken = default,
            params (Expression<Func<T, object>> fieldSelector, Expression<Func<T, object>> valueSelector)[] updateExpressions) where T : class, new()
        {
            return @this.UpdateAsync(index, id.ToString(), cancellationToken, updateExpressions);
        }

        public static ValueTask<bool> UpdateAsync<TDocument, TNestedDocument>(
            this IElasticSearchRepository<TDocument> @this,
            string index,
            Guid id,
            Expression<Func<TDocument, IEnumerable<TNestedDocument>>> childrenSelector,
            Expression<Func<TNestedDocument, bool>> childrenFilter,
            CancellationToken cancellationToken = default,
            params (Expression<Func<TNestedDocument, object>> fieldSelector, Expression<Func<TNestedDocument, object>> valueSelector)[] updateExpressions) where TDocument : class, new()
        {
            return @this.UpdateAsync(index, id.ToString(), childrenSelector, childrenFilter, cancellationToken, updateExpressions);
        }

        public static ValueTask<Exception> UpdateAsync<T>(
            this IElasticSearchRepository<T> @this,
            string index,
            Func<T, Guid> idSelector,
            IEnumerable<T> documents,
            CancellationToken cancellationToken = default) where T : class, new()
        {
            return @this.UpdateAsync(index, (T model) => idSelector?.Invoke(model).ToString(), documents, cancellationToken);
        }

        public static ValueTask<bool> DeleteAsync<T>(
            this IElasticSearchRepository<T> @this,
            string index,
            Guid id,
            CancellationToken cancellationToken = default) where T : class, new()
        {
            return @this.DeleteAsync(index, id.ToString(), cancellationToken);
        }

        public static async ValueTask<(long totalCount, IEnumerable<T> result)> EqualAsync<T>(
            this IReadonlyElasticSearchRepository<T> @this,
            string index,
            IEnumerable<(Expression<Func<T, object>> fieldSelector, object value)> terms,
            Expression<Func<T, object>> sort = null,
            bool? sortDecending = null,
            int size = 10,
            int skip = 0,
            CancellationToken cancellationToken = default) where T : class, new()
        {
            return await @this.EqualRangeAsync(index, terms, null, sort, sortDecending, size, skip, cancellationToken);
        }
    }
}