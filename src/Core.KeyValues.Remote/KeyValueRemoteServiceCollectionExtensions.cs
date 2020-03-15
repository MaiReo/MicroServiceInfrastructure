using Core.KeyValues;
using Core.KeyValues.Remote;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KeyValueRemoteServiceCollectionExtensions
    {
        public static IKeyValueBuilder AddRemote(this IKeyValueBuilder builder, string baseUrl = "http://middleware-keyvalue/api/")
        {
            builder.Services.AddSingleton<IKeyValueStore, RemoteKeyValueStore>();
            builder.AddConfiguration(new RemoteKeyValueConfiguration { BaseUri = new System.Uri(baseUrl) });
            return builder;
        }
    }
}
