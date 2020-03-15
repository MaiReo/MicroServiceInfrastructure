using System.Collections;
using System.Collections.Generic;

namespace System
{
    internal static class ConditionChainExtensions
    {
        public static T If<T>(this T obj, bool condition, Func<T, T> func)
        {
            if (func is null)
            {
                return obj;
            }
            if (condition)
            {
                return func.Invoke(obj);
            }
            return obj;
        }

        public static string JoinWith(this IEnumerable<string> source, string sep) => string.Join(sep, source);
    }
}