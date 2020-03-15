using Core.Messages.Bus;
using Core.PersistentStore;
using Core.ServiceDiscovery;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbstractionServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageBus(this IServiceCollection services,
            string hostName, int port,
            string virtualhostName,
            string exchangeName, string queueName,
            string userName, string password, string hostServiceName = default, bool? queuePerConsumer = default)
        {
            var options = new MessageBusOptions()
            {
                HostName = hostName,
                Port = port,
                VirtualHost = virtualhostName,
                ExchangeName = exchangeName,
                QueueName = queueName,
                UserName = userName,
                Password = password,
                HostServiceName = hostServiceName,
                UseServiceDiscovery = !string.IsNullOrWhiteSpace(hostServiceName),
                QueuePerConsumer = queuePerConsumer
            };
            services.TryAddSingleton<IMessageBusOptions>(options);
            return services;
        }

        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, string serviceName)
        {
            return services.AddServiceDiscovery(o =>
            {
                o.ServiceName = serviceName;
            });
        }

        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, Action<ServiceDiscoveryConfiguration> optionsAction)
        {
            var options = new ServiceDiscoveryConfiguration();
            optionsAction?.Invoke(options);
            services.TryAddSingleton(options);
            return services;
        }

        public static IServiceCollection AddElasticSearch(this IServiceCollection services, string baseUrl, string username = null, string password = null)
        {
            return services.AddElasticSearch(o =>
            {
                o.BaseUrl = baseUrl;
                o.Username = username;
                o.Password = o.Password;
            });
        }

        public static IServiceCollection AddElasticSearch(this IServiceCollection services, Action<ElasticSearchConfiguration> configureOptions)
        {
            var options = new ElasticSearchConfiguration();
            configureOptions?.Invoke(options);
            services.TryAddSingleton(options);
            return services;
        }
    }
}
