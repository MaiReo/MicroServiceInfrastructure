using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Nest;
//using Nest;

namespace Core.PersistentStore.Repositories
{
    public class FakeElasticSearchRepository<T> : IElasticSearchRepository<T> where T : class, new()
    {
        private Dictionary<string, Dictionary<string, T>> _store;

        public FakeElasticSearchRepository()
        {
            _store = new Dictionary<string, Dictionary<string, T>>();
        }

        ///<summary>
        /// 初始化索引的映射
        ///</summary>
        public ValueTask InitAsync(string index, CancellationToken cancellationToken = default)
        {
            return new ValueTask();
        }

        ///<summary>
        /// 获取整个文档
        ///</summary>
        public ValueTask<T> GetAsync(string index, string id, CancellationToken cancellationToken = default)
        {
            T item = default;
            if (_store.TryGetValue(index, out var dic))
            {
                dic.TryGetValue(id, out item);
            }
            return new ValueTask<T>(item);
        }

        ///<summary>
        /// 创建新的文档
        ///</summary>
        public ValueTask<bool> CreateAsync(string index, string id, T item, CancellationToken cancellationToken = default)
        {
            Dictionary<string, T> dic = default;
            lock (_store)
            {
                if (!_store.TryGetValue(index, out dic))
                {
                    dic = new Dictionary<string, T>();
                    _store.TryAdd(index, dic);
                }
            }
            dic.TryAdd(id, item);
            return new ValueTask<bool>(true);
        }

        ///<summary>
        /// 更新文档
        ///</summary>
        public ValueTask<bool> UpdateAsync(string index, string id, T item, CancellationToken cancellationToken = default)
        {
            Dictionary<string, T> dic = default;
            lock (_store)
            {
                if (!_store.TryGetValue(index, out dic))
                {
                    dic = new Dictionary<string, T>();
                    _store.TryAdd(index, dic);
                }
            }
            if (dic.ContainsKey(id))
            {
                dic[id] = item;
                return new ValueTask<bool>(true);
            }
            return new ValueTask<bool>(false);
        }

        public ValueTask<(long totalCount, IEnumerable<T> result)> QueryAsync(
            string index,
            Func<QueryContainerDescriptor<T>, QueryContainer> query,
            Expression<Func<T, object>> sort = null,
            bool? sortDecending = null,
            int size = 10,
            int skip = 0,
            CancellationToken cancellationToken = default)
        {
            var indices = new List<string>
            {
                index,
            };

            if (index.Contains("*"))
            {
                indices = _store.Keys.Where(key => System.Text.RegularExpressions.Regex.IsMatch(key, index)).ToList();
            }

            var source = indices.SelectMany(idx => _store[idx].Values).ToList();

            var pagedQuery = source.AsEnumerable();

            if (sort != null)
            {
                var sortFunc = sort.Compile();
                if (sortDecending == true)
                {
                    pagedQuery = pagedQuery.OrderByDescending(sortFunc);
                }
                else
                {
                    pagedQuery = pagedQuery.OrderBy(sortFunc);
                }
            }
            pagedQuery = pagedQuery.Skip(skip).Take(size);
            return new ValueTask<(long totalCount, IEnumerable<T> result)>((source.Count, pagedQuery));
        }

        public ValueTask<bool> DeleteAsync(string index, string id, CancellationToken cancellationToken = default)
        {
            if (_store.TryGetValue(index, out var dic))
            {
                if (dic.ContainsKey(id))
                {
                    dic.Remove(id);
                    return new ValueTask<bool>(true);
                }
            }
            return new ValueTask<bool>(false);
        }

        public ValueTask<bool> UpdateAsync(string index, string id, CancellationToken cancellationToken = default, params (Expression<Func<T, object>> fieldSelector, Expression<Func<T, object>> valueSelector)[] updateExpressions)
        {
            if (updateExpressions is null)
            {
                throw new ArgumentNullException(nameof(updateExpressions));
            }
            if (_store.TryGetValue(index, out var dic))
            {
                if (dic.ContainsKey(id))
                {
                    var value = dic[id];
                    foreach (var (selector, valExpr) in updateExpressions)
                    {
                        var leftExpr = selector.Body;

                        if (leftExpr.NodeType == ExpressionType.Convert)
                        {
                            leftExpr = ((UnaryExpression)leftExpr).Operand;
                        }
                        var rightExpr = ParameterReplacerVisitor.Replace(valExpr.Body, valExpr.Parameters[0], selector.Parameters[0]);
                        if (rightExpr.NodeType == ExpressionType.Convert)
                        {
                            rightExpr = ((UnaryExpression)rightExpr).Operand;
                        }
                        rightExpr = Expression.Convert(rightExpr, leftExpr.Type);
                        var assignExpr = Expression.Assign(leftExpr, rightExpr);
                        var assignLambda = Expression.Lambda<Action<T>>(assignExpr, selector.Parameters[0]);
                        var assignFunc = assignLambda.Compile();
                        assignFunc.Invoke(value);
                    }
                }
                return new ValueTask<bool>(true);
            }
            return new ValueTask<bool>(false);
        }

        public ValueTask<bool> UpdateAsync<TDocument>(string index, string id, Expression<Func<T, IEnumerable<TDocument>>> childrenSelector, Expression<Func<TDocument, bool>> childrenFilter, CancellationToken cancellationToken = default, params (Expression<Func<TDocument, object>> fieldSelector, Expression<Func<TDocument, object>> valueSelector)[] updateExpressions)
        {
            if (childrenSelector is null)
            {
                throw new ArgumentNullException(nameof(childrenSelector));
            }
            if (_store.TryGetValue(index, out var dic))
            {
                if (dic.ContainsKey(id))
                {
                    var value = dic[id];
                    var children = childrenSelector.Compile().Invoke(value);
                    if (childrenFilter != null)
                    {
                        children = children.Where(childrenFilter.Compile());
                    }

                    var updateActions = new List<Action<TDocument>>();

                    foreach (var (field, val) in updateExpressions)
                    {
                        var leftExpr = field.Body;
                        if (leftExpr.NodeType == ExpressionType.Convert)
                        {
                            leftExpr = ((UnaryExpression)leftExpr).Operand;
                        }
                        var rightExpr = val.Body;
                        if (rightExpr.NodeType == ExpressionType.Convert)
                        {
                            rightExpr = ((UnaryExpression)rightExpr).Operand;
                        }
                        rightExpr = Expression.Convert(rightExpr, leftExpr.Type);
                        var assignExpr = Expression.Assign(leftExpr, rightExpr);

                        var lambdaExpr = Expression.Lambda<Action<TDocument>>(assignExpr, field.Parameters);
                        var action = lambdaExpr.Compile();
                        updateActions.Add(action);
                    }

                    foreach (var child in children)
                    {
                        updateActions.ForEach(action => action.Invoke(child));
                    }
                }
                return new ValueTask<bool>(true);
            }
            return new ValueTask<bool>(false);
        }

        public ValueTask<Exception> CreateAsync(string index, Func<T, string> idSelector, IEnumerable<T> documents, bool replace = false, CancellationToken cancellationToken = default)
        {
            Dictionary<string, T> dic = default;
            lock (_store)
            {
                if (!_store.TryGetValue(index, out dic))
                {
                    dic = new Dictionary<string, T>();
                    _store.TryAdd(index, dic);
                }
            }
            if (replace)
            {
                dic.Clear();
            }
            foreach (var item in documents)
            {
                var id = idSelector(item);
                dic.TryAdd(id, item);
            }
            return new ValueTask<Exception>();
        }

        public ValueTask<Exception> UpdateAsync(string index, Func<T, string> idSelector, IEnumerable<T> documents, CancellationToken cancellationToken = default)
        {
            Dictionary<string, T> dic = default;
            lock (_store)
            {
                if (!_store.TryGetValue(index, out dic))
                {
                    dic = new Dictionary<string, T>();
                    _store.TryAdd(index, dic);
                }
            }
            foreach (var item in documents)
            {
                var id = idSelector(item);
                if (dic.ContainsKey(id))
                {
                    dic[id] = item;
                }
            }
            return new ValueTask<Exception>();
        }

        public ValueTask<(long totalCount, IEnumerable<T> result)> EqualRangeAsync(string index,
          IEnumerable<(Expression<Func<T, object>> fieldSelector, object value)> eq,
          IEnumerable<(Expression<Func<T, DateTimeOffset>> fieldSelector, DateTimeOffset gte, DateTimeOffset lte)> range,
          Expression<Func<T, object>> sort, bool? sortDecending, int size, int skip, CancellationToken cancellationToken)
        {
            var queryExprList = new List<Expression<Func<T, bool>>>();

            if (eq != null)
            {
                foreach (var (fieldSelector, value) in eq)
                {
                    var bodyExpr = fieldSelector.Body;
                    if (bodyExpr.NodeType == ExpressionType.Convert)
                    {
                        bodyExpr = (bodyExpr as UnaryExpression).Operand;
                    }
                    var queryExpr = Expression.Lambda<Func<T, bool>>(Expression.Equal(bodyExpr, Expression.Constant(value, bodyExpr.Type)), fieldSelector.Parameters);
                    queryExprList.Add(queryExpr);
                }
            }

            if (range != null)
            {
                foreach (var (fieldSelector, gte, lte) in range)
                {

                    BinaryExpression left = null;
                    BinaryExpression right = null;
                    Expression body = null;
                    if (gte != default)
                    {
                        left = Expression.GreaterThanOrEqual(fieldSelector.Body, Expression.Constant(gte, typeof(DateTimeOffset)));
                    }
                    if (lte != default)
                    {
                        right = Expression.LessThanOrEqual(fieldSelector.Body, Expression.Constant(lte, typeof(DateTimeOffset)));
                    }

                    if (left != null && right != null)
                    {
                        body = Expression.AndAlso(left, right);
                    }
                    else if (left != null)
                    {
                        body = left;
                    }
                    else if (right != null)
                    {
                        body = right;
                    }
                    else
                    {
                        continue;
                    }
                    var queryExpr = Expression.Lambda<Func<T, bool>>(body, fieldSelector.Parameters);
                    queryExprList.Add(queryExpr);
                }
            }

            var indices = new List<string>
            {
                index,
            };

            if (index.Contains("*"))
            {
                indices = _store.Keys.Where(key => System.Text.RegularExpressions.Regex.IsMatch(key, index)).ToList();
            }

            var queryFuncList = queryExprList.Select(x => x.Compile()).ToList();

            var source = Enumerable.Empty<T>();
            foreach (var idx in indices)
            {
                if (_store.TryGetValue(idx, out var dic))
                {
                    var thisSrc = dic.Values.AsEnumerable();
                    foreach (var queryFunc in queryFuncList)
                    {
                        thisSrc = thisSrc.Where(queryFunc);
                    }
                    source = source.Concat(thisSrc);
                }
            }
            var sourceList = source.ToList();
            var totalCount = sourceList.Count;
            var pagedQuery = sourceList.AsEnumerable();
            if (sort != null)
            {
                var sortFunc = sort.Compile();
                if (sortDecending == true)
                {
                    pagedQuery = pagedQuery.OrderByDescending(sortFunc);
                }
                else
                {
                    pagedQuery = pagedQuery.OrderBy(sortFunc);
                }
            }
            pagedQuery = pagedQuery.Skip(skip).Take(size);
            return new ValueTask<(long totalCount, IEnumerable<T> result)>((sourceList.Count, pagedQuery));
        }
    }
}

internal class ParameterReplacerVisitor : ExpressionVisitor
{
    /// <summary>Expression to replace with.</summary>
    private Expression newExpression;

    /// <summary>Parameter to replace.</summary>
    private ParameterExpression oldParameter;

    /// <summary>Initializes a new <see cref="ParameterReplacerVisitor"> instance.</see></summary>
    /// <param name="oldParameter">Parameter to replace. 
    /// <param name="newExpression">Expression to replace with.
    private ParameterReplacerVisitor(ParameterExpression oldParameter, Expression newExpression)
    {
        this.oldParameter = oldParameter;
        this.newExpression = newExpression;
    }

    /// <summary>
    /// Replaces the occurences of <paramref name="oldParameter"> for <paramref name="newParameter"> in 
    /// <paramref name="expression">.
    /// </paramref></paramref></paramref></summary>
    /// <param name="expression">Expression to perform replacement on.
    /// <param name="oldParameter">Parameter to replace. 
    /// <param name="newExpression">Expression to replace with.
    /// <returns>A new expression with the replacement performed.</returns> 
    internal static Expression Replace(Expression expression, ParameterExpression oldParameter, Expression newExpression)
    {
        Debug.Assert(expression != null, "expression != null");
        Debug.Assert(oldParameter != null, "oldParameter != null");
        Debug.Assert(newExpression != null, "newExpression != null");
        return new ParameterReplacerVisitor(oldParameter, newExpression).Visit(expression);
    }

    /// <summary>ParameterExpression visit method.</summary> 
    /// <param name="p">The ParameterExpression expression to visit 
    /// <returns>The visited ParameterExpression expression </returns>
    protected override Expression VisitParameter(ParameterExpression p)
    {
        if (p == this.oldParameter)
        {
            return this.newExpression;
        }
        else
        {
            return p;
        }
    }
}