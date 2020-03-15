using Core.KeyValues;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KeyValueCoreServiceCollectionExtensions
    {
        public static IKeyValueBuilder AddKeyValueCore(this IServiceCollection services)
        {
            var builder = new KeyValueBuilder(services);
            builder.Services.TryAddSingleton<IKeyValueConfigurationRoot, KeyValueConfigurationRoot>();
            builder.Services.TryAddSingleton<IKeyValueReader, NullKeyValueReader>();
            builder.Services.TryAddSingleton<IKeyValueWriter, NullKeyValueWriter>();
            return builder;
        }
    }
}
