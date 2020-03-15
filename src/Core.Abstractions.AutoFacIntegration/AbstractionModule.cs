                                                                                   using Autofac;
using Core.BackgroundJobs;
using Core.Events.Bus;
using Core.Messages;
using Core.Messages.Bus;
using Core.Messages.Bus.Factories;
using Core.Messages.Bus.Internal;
using Core.Messages.Store;
using Core.Messages.Utilities;
using Core.PersistentStore.Repositories;
using Core.PersistentStore.Uow;
using Core.Pipeline;
using Core.RemoteCall;
using Core.ServiceDiscovery;
using Core.Session;
using Core.Utilities;
using Core.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Core.Abstractions
{
    public sealed class AbstractionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterIfNot<IMessagePublisher, MessagePublisher>(ServiceLifetime.Transient)
                   .RegisterIfNot<IMessageDescriptorResolver, MessageDescriptorResolver>(ServiceLifetime.Transient)
                   .RegisterIfNot<IMessageScopeCreator, MessageScopeCreator>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IMessageHandlerCaller, ExpressionTreeMessageHandlerCaller>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IMessageHandlerFactoryStore, MessageHandlerFactoryStore>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IMessageBus, MessageBus>(ServiceLifetime.Transient)
                   .RegisterIfNot<IMessageConverter, DefaultMessageConverter>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IServiceHelper, DefaultServiceHelper>(ServiceLifetime.Singleton)
                   .RegisterIfNot<HttpMessageHandler, HttpClientHandler>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IHttpClientWrapper, HttpClientWrapper>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IRPCService, RPCService>(ServiceLifetime.Transient)
                   .RegisterIfNot<IBackgroundJobHelper, NullBackgroundJobHelper>(ServiceLifetime.Singleton)
                   .RegisterIfNot<ICoreSession, NullCoreSession>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IServiceHelper, NullServiceHelper>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IServiceDiscoveryHelper, NullServiceDiscoveryHelper>(ServiceLifetime.Singleton)
                   .RegisterIfNot<RandomServiceEndpointSelector>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IServiceEndpointSelector, RandomServiceEndpointSelector>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IMessageHasher, MessageHasher>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IPublishedMessageStore, PublishedMessageStore>(ServiceLifetime.Transient)
                   .RegisterIfNot<IConsumedMessageStore, ConsumedMessageStore>(ServiceLifetime.Transient)
                   .RegisterIfNot<IPublishedMessageStorageProvider, PublishedLoggerMessageStorageProvider>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IConsumedMessageStorageProvider, ConsumedLoggerMessageStorageProvider>(ServiceLifetime.Singleton)
                   .RegisterIfNot<IUnitOfWorkOptions, UnitOfWorkOptions>(ServiceLifetime.Singleton)
                   .RegisterInstanceIfNot<IEventBus>(NullEventBus.Instance)
                   ;

            builder.Register(c => c.Resolve<IHttpClientWrapper>().HttpClient)
                .As<HttpClient>()
                .IfNotRegistered(typeof(HttpClient))
                .SingleInstance()
                .ExternallyOwned();

            //Consul
            builder.RegisterIfNot<ServiceDiscoveryConfiguration>();

            //Repository
            builder.RegisterGeneric(typeof(NullAsyncRepository<,>))
               .As(typeof(IRepository<,>))
               .IfNotRegistered(typeof(IRepository<,>))
               .SingleInstance();

            builder.RegisterGeneric(typeof(NullAsyncRepository<,>))
               .As(typeof(IAsyncRepository<,>))
               .IfNotRegistered(typeof(IAsyncRepository<,>))
               .SingleInstance();

            builder.RegisterType<UnitOfWorkBase>()
                .AsSelf()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(AsyncPipelineAggregator<>))
               .As(typeof(IAsyncPipelineAggregator<>))
               .IfNotRegistered(typeof(IAsyncPipelineAggregator<>))
               .InstancePerLifetimeScope();

            builder.RegisterType<IAsyncPipelineContextHandler>()
                .AsSelf()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();

        }
    }

}
