using System;
using System.Linq;

namespace Core.EnumClasses
{
    public static class EnumClassExtensions
    {
        public static T AsEnum<T>(this string id) where T : class, IEnumClass, new()
        {
            return new T().GetAll().SingleOrDefault(x => string.Equals(id, x.Id, StringComparison.InvariantCultureIgnoreCase)) as T;
        }
    }
}