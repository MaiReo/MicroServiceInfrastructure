using Core.KeyValues;
using Core.KeyValues.Json;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KeyValueJsonServiceCollectionExtensions
    {
        [Obsolete("Use AddKeyPassThroughValueWrapedJson instead")]
        public static IKeyValueBuilder AddNestedJson(this IKeyValueBuilder builder)
        {
            builder.Services.AddSingleton<IKeyValueReader, NestedJsonKeyValueReaderWriter>();
            builder.Services.AddSingleton<IKeyValueWriter, NestedJsonKeyValueReaderWriter>();
            return builder;
        }

        public static IKeyValueBuilder AddKeyPassThroughValueWrapedJson(this IKeyValueBuilder builder)
        {
            builder.Services.AddSingleton<IKeyValueReader, KeyPassThroughValueWrapedAsJsonObjectKeyValueReaderWriter>();
            builder.Services.AddSingleton<IKeyValueWriter, KeyPassThroughValueWrapedAsJsonObjectKeyValueReaderWriter>();
            return builder;
        }
        
    }
}
