using System;
using System.Net;

namespace Core.Extensions
{
    public static class PrimitiveTypeExtensions
    {
        public static Guid? AsGuidOrNull(this string @this)
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return default;
            }
            return Guid.TryParse(@this, out var value) ? value : default(Guid?);
        }

        public static Guid AsGuidOrDefault(this string @this)
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return default;
            }
            return Guid.TryParse(@this, out var value) ? value : default;
        }

        public static string AsStringOrDefault(this Guid @this)
        {
            if (@this == default)
            {
                return default;
            }
            return @this.ToString();
        }


        public static DateTime? AsDateTimeOrDefault(this string @this)
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return default;
            }
            return DateTimeOffset.TryParse(@this, out var value) ? value.UtcDateTime : default;
        }

        public static string AsISO8601StringOrDefault(this DateTime? @this)
        {
            if (@this == default)
            {
                return default;
            }
            return new DateTimeOffset(@this.Value).ToString("o");
        }

        public static DateTimeOffset? AsDateTimeOffsetOrDefault(this string @this)
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return default;
            }
            return DateTimeOffset.TryParse(@this, out var value) ? value : default;
        }

        public static string AsISO8601StringOrDefault(this DateTimeOffset? @this)
        {
            if (@this == default)
            {
                return default;
            }
            return @this.Value.ToString("o");
        }

        public static string ToUrlEncodedString(this string @this)
        {
            if (string.IsNullOrWhiteSpace(@this))
            {
                return default;
            }
            return WebUtility.UrlEncode(@this);
        }
    }
}
