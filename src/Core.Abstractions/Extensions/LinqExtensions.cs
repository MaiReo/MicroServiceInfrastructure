namespace Core.Linq.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class LinqQueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> expression)
        {
            return condition ? source.Where(expression) : source;
        }

        public static IQueryable<T> WhereIfNot<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> expression)
        {
            return condition ? source : source.Where(expression);
        }
    }
}

namespace Core.Collection.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class LinqEnumerableExtensions
    {
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> expression)
        {
            return condition ? source.Where(expression) : source;
        }

        public static IEnumerable<T> WhereIfNot<T>(this IEnumerable<T> source, bool condition, Func<T, bool> expression)
        {
            return condition ? source : source.Where(expression);
        }
    }
}