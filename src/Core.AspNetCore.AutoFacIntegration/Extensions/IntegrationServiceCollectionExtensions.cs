using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core.Abstractions;
using Core.Messages;
using Core.PersistentStore;
using Core.ServiceDiscovery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IntegrationServiceCollectionExtensions
    {
        public static IServiceCollection RegisterRequiredServices(
            this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddDistributedMemoryCache();
            services.AddHealthChecks();

            var keyValueBaseUrl = Configuration["KeyValue:BaseUrl"];
            if (string.IsNullOrWhiteSpace(keyValueBaseUrl))
            {
                keyValueBaseUrl = "http://middleware-keyvalue/api/";
            }

            services.AddKeyValue(keyValueBaseUrl);

            services.AddMessageBus(
                Configuration["MessageQueue:HostName"],
                Configuration.GetValue("MessageQueue:Port", 0),
                Configuration["MessageQueue:VirtualHostName"],
                Configuration["MessageQueue:ExchangeName"],
                Configuration["MessageQueue:QueueName"],
                Configuration["MessageQueue:UserName"],
                Configuration["MessageQueue:Password"],
                Configuration["MessageQueue:ServiceName"],
                Configuration.GetValue("MessageQueue:QueuePerConsumer", default(bool?)));

            services.AddServiceDiscovery(Configuration["ServiceDiscovery:ServiceName"]);

            services.AddElasticSearch(
                Configuration["ElasticSearchServiceName"],
                Configuration["ElasticSearchUsername"],
                Configuration["ElasticSearchPassword"]);

            return services;
        }



    }
}
