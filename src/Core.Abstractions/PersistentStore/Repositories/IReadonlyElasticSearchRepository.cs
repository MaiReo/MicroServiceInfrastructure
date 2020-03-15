using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.PersistentStore.Repositories
{
    public interface IReadonlyElasticSearchRepository<T> where T : class, new()
    {
        ///<summary>
        /// 获取整个文档
        ///</summary>
        ValueTask<T> GetAsync(string index, string id, CancellationToken cancellationToken = default);

        ///<summary>
        /// 等值加范围查找文档
        ///</summary>
        ValueTask<(long totalCount, IEnumerable<T> result)> EqualRangeAsync(string index,
        IEnumerable<(Expression<Func<T, object>> fieldSelector, object value)> eq = null,
        IEnumerable<(Expression<Func<T, DateTimeOffset>> fieldSelector, DateTimeOffset gte, DateTimeOffset lte)> range = null,
        Expression<Func<T, object>> sort = null, bool? sortDecending = null, int size = 10, int skip = 0,
        CancellationToken cancellationToken = default);
    }
}