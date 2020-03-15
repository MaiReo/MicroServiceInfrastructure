using System;
using System.Linq;
using Core.KeyValues;
using Core.KeyValues.AliyunACM;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AliyunACMKeyValueExtensions
    {
        public static IKeyValueBuilder AddAliyunACM(this IKeyValueBuilder builder, string endpoint,
            string @namespace,
            string ramRoleName = default,
            string accessKey = default,
            string secretKey = default)
        {
            var configuration = new AliyunACMKeyValueConfiguration(endpoint, @namespace, ramRoleName, accessKey, secretKey);
            builder.AddConfiguration(configuration);
            builder.Services.TryAddSingleton<IKeyValueStore, AliyunACMKeyValueStore>();
            return builder;
        }
    }
}

namespace Core.KeyValues.AliyunACM.Internal
{
    internal static class AliyunACMKeyValueExtensions
    {
        internal static AliyunACMKeyValueConfiguration GetConfiguration(this IKeyValueConfigurationRoot configurationRoot)
        {
            var configuration = configurationRoot?.Configuration?.OfType<AliyunACMKeyValueConfiguration>()?.FirstOrDefault();
            if (configuration == default)
            {
                throw new InvalidOperationException("尚未初始化阿里云ACM必要配置");
            }
            configuration.Validate();
            return configuration;
        }
    }
}