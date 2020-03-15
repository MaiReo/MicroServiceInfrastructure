using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nest;

namespace Core.PersistentStore.Repositories
{
    public class ElasticSearchRepository<T> : IElasticSearchRepository<T> where T : class, new()
    {
        private readonly ElasticSearchConfiguration _configuration;

        private readonly ILogger _logger;

        public ElasticSearchRepository(
            ElasticSearchConfiguration configuration,
            ILogger<ElasticSearchRepository<T>> logger = null)
        {
            _configuration = configuration;
            _logger = (ILogger)logger ?? NullLogger.Instance;
        }

        public async ValueTask<bool> CreateAsync(string index, string id, T item, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                throw new ArgumentNullException(nameof(index), "缺少文档索引");
            }

            var es = CreateElasticSearchClient();

            var createIndexFlag = await CreateIndexIfNotExistsAsync(es, index, cancellationToken);

            if (!createIndexFlag)
            {
                return false;
            }

            var response = await es.CreateAsync(item,
                doc => doc.Index(index)
                        .Id(id)
                        .Refresh(Elasticsearch.Net.Refresh.True),
                cancellationToken);

            if (!response.IsValid)
            {
                _logger.LogError("---------create document error begin--------");
                _logger.LogError(response.DebugInformation);
                _logger.LogError("---------create document error end---------");
                return false;
            }

            return true;
        }

        private async ValueTask<bool> CreateIndexIfNotExistsAsync(ElasticClient es, string index, CancellationToken cancellationToken)
        {
            var existResponse = await es.IndexExistsAsync(index, cancellationToken: cancellationToken);
            if (!existResponse.IsValid)
            {
                _logger.LogError("---------check index error begin-------------------");
                _logger.LogError(existResponse.DebugInformation);
                _logger.LogError("---------check index error end---------------------");
                return false;
            }
            if (existResponse.Exists)
            {
                return true;
            }
            var indexResponse = await es.CreateIndexAsync(index,
                c => c.Mappings(ms => ms.Map<T>(m => m.AutoMap())),
                cancellationToken: cancellationToken);
            if (!indexResponse.IsValid)
            {
                _logger.LogError("---------create index error begin-------------------");
                _logger.LogError(indexResponse.DebugInformation);
                _logger.LogError("---------create index error end---------------------");
                return false;
            }
            return true;
        }

        public async ValueTask<bool> UpdateAsync(string index, string id, T item, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                throw new ArgumentNullException(nameof(index), "缺少文档索引");
            }

            var es = CreateElasticSearchClient();
            var response = await es.UpdateAsync<T>(
                    id,
                    (doc) => doc.Index(index).Doc(item).Refresh(Elasticsearch.Net.Refresh.True),
                    cancellationToken: cancellationToken);
            if (!response.IsValid)
            {
                _logger.LogError("---------update document error begin-----------------");
                _logger.LogError(response.DebugInformation);
                _logger.LogError("---------update document error end-------------------");
                return false;
            }
            return true;
        }

        public async ValueTask<T> GetAsync(string index, string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                throw new ArgumentNullException(nameof(index), "缺少文档索引");
            }
            var es = CreateElasticSearchClient();
            var response = await es.GetAsync<T>(
                    id,
                    doc => doc.Index(index),
                    cancellationToken: cancellationToken);
            if (!response.IsValid)
            {
                if (!response.Found)
                {
                    return default;
                }
                if (response.ServerError?.Status == 404 || response.ServerError?.Error.Type == "index_not_found_exception")
                {
                    _logger.LogWarning($"Get document failed, reason: {response.ServerError.Error.Reason}, index: [{index}]");
                    return default;
                }
                _logger.LogError("---------get document error begin-----------------");
                _logger.LogError(response.DebugInformation);
                _logger.LogError("---------get document error end-------------------");
                return default;

            }
            return response.Source;
        }

        public virtual ElasticClient CreateElasticSearchClient()
        {
            var settings = GetConnectionSettings();
            var es = new ElasticClient(settings);
            return es;
        }

        protected virtual ConnectionSettings GetConnectionSettings()
        {
            if (string.IsNullOrWhiteSpace(_configuration.BaseUrl))
            {
                throw new InvalidOperationException("未配置ElasticSearch地址");
            }
            var uri = new Uri(_configuration.BaseUrl);
            var settings = new ConnectionSettings(uri);
            if ((!string.IsNullOrWhiteSpace(_configuration.Username)) && (!string.IsNullOrWhiteSpace(_configuration.Password)))
            {
                _logger.LogDebug("配置ElasticSearch连接使用了用户名和密码");
                settings = settings.BasicAuthentication(_configuration.Username, _configuration.Password);
            }
            return settings;
        }

        public async ValueTask InitAsync(string index, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Try Initializing mapping for index [{index}], document model clr type: [{typeof(T).FullName}]");
            var es = CreateElasticSearchClient();
            var createIndexFlag = await CreateIndexIfNotExistsAsync(es, index, cancellationToken);
            if (!createIndexFlag)
            {
                _logger.LogInformation($"An error occurred during initializing mapping for index [{index}]");
            }
        }

        public async ValueTask<(long totalCount, IEnumerable<T> result)> QueryAsync(
            string index,
            Func<QueryContainerDescriptor<T>, QueryContainer> query,
            Expression<Func<T, object>> sort = null,
            bool? sortDecending = null,
            int size = 10,
            int skip = 0,
            CancellationToken cancellationToken = default)
        {
            var es = CreateElasticSearchClient();
            var response = await es.SearchAsync<T>(s =>
            {
                var desc = s.Index(index).Skip(skip).Size(size);
                if (query != null)
                {
                    desc = desc.Query(query);
                }
                if (sort != null)
                {
                    if (sortDecending == true)
                    {
                        desc = desc.Sort(x => x.Descending(sort));
                    }
                    else
                    {
                        desc = desc.Sort(x => x.Ascending(sort));
                    }
                }
                return desc;
            }, cancellationToken);
            if (!response.IsValid)
            {
                if (response.ServerError?.Status == 404 || response.ServerError?.Error.Type == "index_not_found_exception")
                {
                    _logger.LogWarning($"Search document failed, reason: {response.ServerError?.Error.Reason}, index: [{index}]");
                    return (0, Enumerable.Empty<T>());
                }
                _logger.LogError("---------search document error begin-----------------");
                _logger.LogError(response.DebugInformation);
                _logger.LogError("---------search document error end-------------------");
                throw new ElasticSearchOperationIsNotValidException("查询出错", index, response.OriginalException);
            }
            return (response.Total, response.Hits.Select(x => x.Source));
        }

        public async ValueTask<bool> DeleteAsync(string index, string id, CancellationToken cancellationToken = default)
        {
            var es = CreateElasticSearchClient();
            var response = await es.DeleteAsync<T>(id, q => q.Index(index), cancellationToken);
            if (!response.IsValid)
            {
                if (response.ServerError?.Status == 404 || response.ServerError?.Error?.Type == "index_not_found_exception")
                {
                    _logger.LogWarning($"delete document failed, reason: {response.ServerError?.Error?.Reason}, index: [{index}]");
                    return true;
                }
                _logger.LogError("---------delete document error begin-----------------");
                _logger.LogError(response.DebugInformation);
                _logger.LogError("---------delete document error end-------------------");
                return false;
            }
            return true;
        }

        public async ValueTask<bool> UpdateAsync(string index, string id, CancellationToken cancellationToken = default, params (Expression<Func<T, object>> fieldSelector, Expression<Func<T, object>> valueSelector)[] updateExpressions)
        {
            if (updateExpressions is null)
            {
                throw new ArgumentNullException(nameof(updateExpressions));
            }
            T doc = null;
            var esUpdateParams = new Dictionary<string, object>();
            var esUpdateScript = new StringBuilder();
            var sumAddAssignScripts = new List<(string sourceProp, string script)>();
            var sumAssignScripts = new List<string>();
            var sumIndex = 0;
            foreach (var (fieldSelector, valueSelector) in updateExpressions)
            {
                var fieldName = _PopulateMemberName(fieldSelector.Body);

                var valueExpr = valueSelector.Body;

                if (valueExpr.NodeType == ExpressionType.Convert)
                {
                    valueExpr = (valueExpr as UnaryExpression).Operand;
                }

                if (valueExpr.NodeType == ExpressionType.Call)
                {
                    var callExpr = (valueExpr as MethodCallExpression);
                    if (!callExpr.Method.IsStatic)
                    {
                        throw new NotSupportedException("暂不支持复杂结构的值表达式");
                    }
                    if (callExpr.Method.DeclaringType != typeof(System.Linq.Enumerable))
                    {
                        throw new NotSupportedException("暂不支持Linq以外的值表达式");
                    }
                    if (callExpr.Method.Name == "Sum")
                    {
                        sumIndex++;
                        string valueSumSourceName = null;
                        string valueSumMemberName = null;

                        var sourceExpr = callExpr.Arguments.ElementAt(0);
                        if (sourceExpr.NodeType == ExpressionType.MemberAccess)
                        {
                            valueSumSourceName = (sourceExpr as MemberExpression).Member.Name;
                        }
                        var sumMemberExpr = callExpr.Arguments.ElementAt(1);
                        if (sumMemberExpr.NodeType == ExpressionType.Lambda)
                        {
                            sumMemberExpr = (sumMemberExpr as LambdaExpression).Body;
                        }
                        if (sumMemberExpr.NodeType == ExpressionType.MemberAccess)
                        {
                            valueSumMemberName = (sumMemberExpr as MemberExpression).Member.Name;
                        }
                        sumAddAssignScripts.Add((valueSumSourceName, $"sum{sumIndex} += ctx._source.{valueSumSourceName}[i].{valueSumMemberName};"));
                        sumAssignScripts.Add($"ctx._source.{fieldName} = sum{sumIndex};");

                    }
                    else
                    {
                        throw new NotSupportedException("暂不支持Linq的Sum以外的值表达式");
                    }
                }
                else
                {
                    if (doc is null)
                    {
                        doc = await GetAsync(index, id, cancellationToken);
                    }
                    if (doc is null)
                    {
                        return false;
                    }
                    //Enumerates value directly (not recommend)
                    var value = valueSelector.Compile().Invoke(doc);
                    esUpdateParams.Add(fieldName, value);
                    var assignScript = $"ctx._source.{fieldName} = params.{fieldName};";
                    esUpdateScript.AppendLine(assignScript);
                }
            }

            foreach (var allocScript in Enumerable.Range(1, sumIndex).Select(i => $"float sum{i} = 0f;"))
            {
                esUpdateScript.AppendLine(allocScript);
            }
            foreach (var g in sumAddAssignScripts.GroupBy(x => x.sourceProp))
            {
                esUpdateScript.AppendLine($"for(int i = 0; i< ctx._source.{g.Key}.length; i++){{");
                foreach (var addSignScript in g)
                {
                    esUpdateScript.Append("\t");
                    esUpdateScript.AppendLine(addSignScript.script);
                }
                esUpdateScript.AppendLine("}");
            }
            foreach (var script in sumAssignScripts)
            {
                esUpdateScript.AppendLine(script);
            }

            var esUpdateScriptStr = esUpdateScript.ToString();

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Updating document by script, index: {index}, id: {id}, script:{Environment.NewLine}{esUpdateScriptStr}");
#endif
            _logger.LogDebug($"Updating document by script, index: {index}, id: {id}, script:{Environment.NewLine}{esUpdateScriptStr}");

            var es = CreateElasticSearchClient();
            var response = await es.UpdateAsync<T>(id, d => d.Index(index).Script(s => s.Source(esUpdateScriptStr)), cancellationToken);
            if (!response.IsValid)
            {
                _logger.LogError("---------update document by script error begin-----------------");
                _logger.LogError(response.DebugInformation);
                _logger.LogError("---------update document by script error end-------------------");
                return false;
            }
            return true;
        }

        private string _PopulateMemberName(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return (expression as MemberExpression).Member.Name;
            }
            if (expression.NodeType == ExpressionType.Convert)
            {
                return _PopulateMemberName((expression as UnaryExpression).Operand);
            }
            return string.Empty;
        }

        private object _EnumerateValue(Expression expression)
        {
            var valueExpr = expression;
            if (valueExpr.Type != typeof(object))
            {
                valueExpr = Expression.Convert(valueExpr, typeof(object));
            }
            try
            {
                var getValueLambda = Expression.Lambda<Func<object>>(valueExpr);
                var getValueFunc = getValueLambda.Compile();
                return getValueFunc.Invoke();
            }
            catch (System.Exception e)
            {
                throw new NotSupportedException("无法枚举表达式的值", e);
            }
        }

        public async ValueTask<bool> UpdateAsync<TDocument>(string index, string id, Expression<Func<T, IEnumerable<TDocument>>> childrenSelector, Expression<Func<TDocument, bool>> childrenFilter, CancellationToken cancellationToken = default, params (Expression<Func<TDocument, object>> fieldSelector, Expression<Func<TDocument, object>> valueSelector)[] updateExpressions)
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                throw new ArgumentNullException(nameof(index), "缺少文档索引");
            }
            if (childrenSelector is null)
            {
                throw new ArgumentNullException(nameof(childrenSelector), "缺少嵌套集合选择器");
            }
            var es = CreateElasticSearchClient();

            var childrenName = _PopulateMemberName(childrenSelector.Body);

            if (string.IsNullOrWhiteSpace(childrenName))
            {
                throw new NotSupportedException("暂不支持复杂结构的选择表达式");
            }

            var esUpdateParams = new Dictionary<string, object>();
            var esUpdateScript = new StringBuilder();
            esUpdateScript.AppendLine($"for(int i = 0; i< ctx._source.{childrenName}.length; i++){{");
            if (childrenFilter != null)
            {
                esUpdateScript.Append("\t");
                if (childrenFilter.Body is BinaryExpression binExpr)
                {
                    var operatorStr = "";

                    var memberExprAtLeft = binExpr.Left.NodeType == ExpressionType.MemberAccess && typeof(TDocument).IsAssignableFrom((binExpr.Left as MemberExpression).Member.DeclaringType);

                    var memberExpr = memberExprAtLeft ? binExpr.Left as MemberExpression : binExpr.Left as MemberExpression;

                    var valueExpr = memberExprAtLeft ? binExpr.Right : binExpr.Left;

                    if (memberExpr is null)
                    {
                        throw new NotSupportedException("表达式左侧或者右侧必须是成员访问表达式");
                    }

                    var filterFieldName = memberExpr.Member.Name;

                    switch (binExpr.NodeType)
                    {
                        case ExpressionType.Equal:
                            operatorStr = "==";
                            break;
                        case ExpressionType.NotEqual:
                            operatorStr = "!=";
                            break;
                        case ExpressionType.GreaterThanOrEqual:
                            operatorStr = memberExprAtLeft ? ">=" : "<=";
                            break;
                        case ExpressionType.GreaterThan:
                            operatorStr = memberExprAtLeft ? ">" : "<";
                            break;
                        case ExpressionType.LessThanOrEqual:
                            operatorStr = memberExprAtLeft ? "<=" : ">=";
                            break;
                        case ExpressionType.LessThan:
                            operatorStr = memberExprAtLeft ? "<" : ">";
                            break;
                        default:
                            throw new NotSupportedException("只支持等于,不等于,大于,大于等于,小于,小于等于表达式");
                    }

                    object filterValue = _EnumerateValue(valueExpr);
                    esUpdateScript.Append($"if(ctx._source.{childrenName}[i].{filterFieldName} {operatorStr} ");
                    if (filterValue is null)
                    {
                        esUpdateScript.Append("null");
                    }
                    else
                    {
                        esUpdateScript.Append("params.filter_name");
                        esUpdateParams.Add("filter_name", filterValue);
                    }
                    esUpdateScript.AppendLine("){");

                    foreach (var (updateField, updateValue) in updateExpressions)
                    {
                        var newValue = _EnumerateValue(updateValue.Body);
                        var fieldName = _PopulateMemberName(updateField.Body);
                        esUpdateScript.Append("\t\t");
                        esUpdateScript.AppendLine($"ctx._source.{childrenName}[i].{fieldName} = params.{fieldName};");
                        esUpdateParams.Add(fieldName, newValue);
                    }
                    // end of "if" block
                    esUpdateScript.AppendLine("\t}");

                }
                else
                {
                    throw new NotSupportedException("暂不支持复杂结构的筛选表达式");
                }
            }
            //end of "for" block
            esUpdateScript.Append("}");

            var esUpdateScriptStr = esUpdateScript.ToString();

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Updating document by script, index: {index}, id: {id}, script:{Environment.NewLine}{esUpdateScriptStr}");
#endif

            _logger.LogDebug($"Updating document by script, index: {index}, id: {id}, script:{Environment.NewLine}{esUpdateScript}");

            var response = await es.UpdateAsync<T>(id, doc => doc.Index(index).Script(s => s.Source(esUpdateScriptStr).Params(esUpdateParams)), cancellationToken);
            if (!response.IsValid)
            {
                _logger.LogError("---------update document by script error begin-----------------");
                _logger.LogError(response.DebugInformation);
                _logger.LogError("---------update document by script error end-------------------");
                return false;
            }
            return true;
        }

        public async ValueTask<Exception> UpdateAsync(string index, Func<T, string> idSelector, IEnumerable<T> documents, CancellationToken cancellationToken = default)
        {
            var exceptionList = new List<ElasticSearchOperationIsNotValidException>();

            if (string.IsNullOrWhiteSpace(index))
            {
                throw new ArgumentNullException(nameof(index), "缺少文档索引");
            }
            if (idSelector is null)
            {
                throw new ArgumentNullException(nameof(idSelector), "缺少主键选择器");
            }
            var es = CreateElasticSearchClient();

            foreach (var item in documents)
            {
                var id = idSelector(item);
                var response = await es.UpdateAsync<T>(
                    id,
                    (doc) => doc.Index(index).Doc(item),
                    cancellationToken: cancellationToken);
                if (!response.IsValid)
                {
                    if (response.ServerError?.Status == 404 || response.ServerError?.Error.Type == "index_not_found_exception")
                    {
                        _logger.LogWarning($"update document failed, reason: {response.ServerError.Error.Reason}, index: [{index}]");
                    }
                    else
                    {
                        exceptionList.Add(new ElasticSearchDocumentUpdateException<string>("文档更新失败", index, id, item, response.OriginalException));
                    }
                }
            }

            var refreshResponse = await es.RefreshAsync(index, cancellationToken: cancellationToken);
            if (!refreshResponse.IsValid)
            {
                if (refreshResponse.ServerError?.Status == 404 || refreshResponse.ServerError?.Error.Type == "index_not_found_exception")
                {
                    _logger.LogWarning($"refresh document failed, reason: {refreshResponse.ServerError.Error.Reason}, index: [{index}]");
                }
                else
                {
                    exceptionList.Add(new ElasticSearchIndexRefreshException("文档索引刷新失败", index, refreshResponse.OriginalException));
                }
            }
            if (exceptionList.Any())
            {
                return new ElasticSearchOperationIsNotValidException("更新文档过程中发生一个或多个错误", exceptionList);
            }
            return null;
        }

        public async ValueTask<Exception> CreateAsync(string index, Func<T, string> idSelector, IEnumerable<T> documents, bool replace = false, CancellationToken cancellationToken = default)
        {
            var exceptionList = new List<ElasticSearchOperationIsNotValidException>();

            if (string.IsNullOrWhiteSpace(index))
            {
                throw new ArgumentNullException(nameof(index), "缺少文档索引");
            }
            if (idSelector is null)
            {
                throw new ArgumentNullException(nameof(idSelector), "缺少主键选择器");
            }
            
            var es = CreateElasticSearchClient();

            if (replace)
            {
                await es.DeleteIndexAsync(index, cancellationToken: cancellationToken);
            }

            var createIndexFlag = await CreateIndexIfNotExistsAsync(es, index, cancellationToken);

            if (!createIndexFlag)
            {
                return new ElasticSearchOperationIsNotValidException("创建索引失败", index);
            }

            foreach (var item in documents)
            {
                var id = idSelector(item);
                var response = await es.CreateAsync(item,
                    doc => doc.Index(index)
                       .Id(id)
                       ,
                    cancellationToken);
                if (!response.IsValid)
                {
                    exceptionList.Add(new ElasticSearchDocumentCreateException<string>("文档创建失败", index, id, item, response.OriginalException));
                }
            }
            var refreshResponse = await es.RefreshAsync(index, cancellationToken: cancellationToken);
            if (!refreshResponse.IsValid)
            {
                exceptionList.Add(new ElasticSearchIndexRefreshException("文档索引刷新失败", index, refreshResponse.OriginalException));
            }
            if (exceptionList.Any())
            {
                return new ElasticSearchOperationIsNotValidException("创建文档过程中发生一个或多个错误", exceptionList);
            }
            return null;
        }

        public async ValueTask<(long totalCount, IEnumerable<T> result)> EqualRangeAsync(string index,
        IEnumerable<(Expression<Func<T, object>> fieldSelector, object value)> eq,
        IEnumerable<(Expression<Func<T, DateTimeOffset>> fieldSelector, DateTimeOffset gte, DateTimeOffset lte)> range,
        Expression<Func<T, object>> sort, bool? sortDecending, int size, int skip,
        CancellationToken cancellationToken)
        {
            var queries = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
            if (range != null)
            {
                foreach (var (fieldSelector, gte, lte) in range)
                {
                    Func<QueryContainerDescriptor<T>, QueryContainer> q =
                      f => f.DateRange(r => r.Field(fieldSelector)
                          .If(!default(DateTimeOffset).Equals(gte), x => x.GreaterThanOrEquals(DateMath.Anchored(gte.Date).RoundTo(DateMathTimeUnit.Day)))
                          .If(!default(DateTimeOffset).Equals(lte), x => x.LessThanOrEquals(DateMath.Anchored(lte.Date).RoundTo(DateMathTimeUnit.Day)))
                          .TimeZone("+08:00"));
                    queries.Add(q);
                }
            }
            if (eq != null)
            {
                foreach (var (fieldSelector, value) in eq)
                {
                    Func<QueryContainerDescriptor<T>, QueryContainer> q = f => f.Term(fieldSelector, value);
                    queries.Add(q);
                }
            }
            Func<QueryContainerDescriptor<T>, QueryContainer> query = null;
            if (queries.Any())
            {
                query = f => +f.Bool(bo => bo.Must(queries));
            }
            return await QueryAsync(index, query, sort, sortDecending, size, skip, cancellationToken);
        }
    }
}