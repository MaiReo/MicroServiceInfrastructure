using Core.KeyValues;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KeyValueServiceCollectionExtensions
    {
        public static void AddKeyValue(
            this IServiceCollection services,
            string baseUrl = "http://middleware-keyvalue/api/")
        {
            var builder = services.AddKeyValueCore();
            builder.AddKeyPassThroughValueWrapedJson();
            builder.AddRemote(baseUrl);
        }
    }
}
