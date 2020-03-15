using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.PersistentStore.Repositories
{
    public interface IElasticSearchRepository<T> : IReadonlyElasticSearchRepository<T> where T : class, new()
    {

        ///<summary>
        /// 初始化索引的映射
        ///</summary>
        ValueTask InitAsync(string index, CancellationToken cancellationToken = default);

        ///<summary>
        /// 创建新的文档
        ///</summary>
        ValueTask<bool> CreateAsync(string index, string id, T item, CancellationToken cancellationToken = default);

        ///<summary>
        /// 创建多个文档
        ///</summary>
        ValueTask<Exception> CreateAsync(string index, Func<T, string> idSelector, IEnumerable<T> documents, bool replace = false, CancellationToken cancellationToken = default);

        ///<summary>
        /// 更新文档
        ///</summary>
        ValueTask<bool> UpdateAsync(string index, string id, T item, CancellationToken cancellationToken = default);

        ///<summary>
        /// 部分更新文档
        ///</summary>
        ValueTask<bool> UpdateAsync(string index, string id,
            CancellationToken cancellationToken = default,
            params (Expression<Func<T, object>> fieldSelector, Expression<Func<T, object>> valueSelector)[] updateExpressions);

        ///<summary>
        /// 部分更新嵌套文档
        ///</summary>
        ValueTask<bool> UpdateAsync<TDocument>(string index, string id,
            Expression<Func<T, IEnumerable<TDocument>>> childrenSelector,
            Expression<Func<TDocument, bool>> childrenFilter,
            CancellationToken cancellationToken = default,
            params (Expression<Func<TDocument, object>> fieldSelector, Expression<Func<TDocument, object>> valueSelector)[] updateExpressions
            );
        
        ValueTask<Exception> UpdateAsync(string index, Func<T, string> idSelector, IEnumerable<T> documents, CancellationToken cancellationToken = default);       

        ///<summary>
        /// 删除文档
        ///</summary>
        ValueTask<bool> DeleteAsync(string index, string id, CancellationToken cancellationToken = default);
    }
}