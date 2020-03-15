using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Core.PersistentStore
{
    public static class ESTools
    {
        public static IEnumerable<(Expression<Func<T, object>> fieldSelector, object value)> Equal<T>(params (Expression<Func<T, object>> fieldSelector, object value)[] terms) => terms;
        public static IEnumerable<(Expression<Func<T, DateTimeOffset>> fieldSelector, DateTimeOffset gte, DateTimeOffset lte)> Range<T>(params (Expression<Func<T, DateTimeOffset>> fieldSelector, DateTimeOffset gte, DateTimeOffset lte)[] terms) => terms;
    }
}