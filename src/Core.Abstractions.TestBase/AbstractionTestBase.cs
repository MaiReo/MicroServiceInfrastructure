using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core.Messages;
using Core.Messages.Bus;
using Core.Messages.Bus.Extensions;
using Core.Messages.Fake;
using Core.PersistentStore.Repositories;
using Core.ServiceDiscovery;
using Core.Session;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.TestBase
{
    public abstract class AbstractionTestBase<TStartup> : IDisposable where TStartup : class
    {

        private IContainer _iocContainer;
        private readonly ICoreSessionProvider _coreSessionProvider;

        protected IComponentContext IocResolver => _iocContainer;

        protected ICoreSession Session => _coreSessionProvider.Session;

        public AbstractionTestBase()
        {
            _iocContainer = RegisterRequiredServices(new ServiceCollection()).Build();
            _coreSessionProvider = Resolve<ICoreSessionProvider>();
            ConstructProperties();
            LoginAsDefaultUser();
        }

        private void LoginAsDefaultUser()
        {
            LoginAs(TestConsts.CURRENT_USER_ID, TestConsts.CURRENT_USER_NAME);
        }

        protected void LoginAs(string userId, string userName)
        {
            Resolve<UnitTestCurrentUser>().Set(userId, userName);
        }

        public IMessageBus MessageBus { get; private set; }

        protected internal virtual ContainerBuilder RegisterRequiredServices(IServiceCollection services)
        {
            var startupAssembly = typeof(TStartup).Assembly;
            services.AddDistributedMemoryCache();
            services.AddMessageBus(TestConsts.MESSAGE_BUS_HOST, 0, TestConsts.MESSAGE_BUS_VHOST, TestConsts.MESSAGE_BUS_EXCHANGE, TestConsts.MESSAGE_BUS_QUEUE, TestConsts.MESSAGE_BUS_USER, TestConsts.MESSAGE_BUS_PWD);
            services.AddServiceDiscovery(o => o.Address = ServiceDiscoveryConfiguration.DEFAULT_ADDRESS);
            services.AddKeyValueCore().AddKeyPassThroughValueWrapedJson().AddInMemory();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.AddCoreModules();
            var thisAssembly = typeof(AbstractionTestBase<>).Assembly;
            var runtimeThisAssembly = GetType().Assembly;
            containerBuilder.RegisterAssemblyByConvention(startupAssembly);
            containerBuilder.RegisterAssemblyByConvention(thisAssembly);
            if (runtimeThisAssembly != startupAssembly)
            {
                containerBuilder.RegisterAssemblyByConvention(runtimeThisAssembly);
            }

            containerBuilder.RegisterGeneric(typeof(FakeElasticSearchRepository<>))
            .AsImplementedInterfaces()
            .SingleInstance();


            containerBuilder
               .RegisterType<NullServiceDiscoveryHelper>()
               .AsSelf()
               .As<IServiceDiscoveryHelper>()
               .SingleInstance();

            containerBuilder
               .RegisterType<NullMessageSubscriber>()
               .AsSelf()
               .As<IMessageSubscriber>()
               .SingleInstance();


            containerBuilder
                .RegisterType<FakeMessagePublisherWrapper>()
                .As<IMessagePublisherWrapper>()
                .AsSelf()
                .SingleInstance();

            containerBuilder
                .RegisterType<UnitTestCoreSessionProvider>()
                .AsSelf()
                .As<ICoreSessionProvider>()
                .SingleInstance();

            containerBuilder
                .RegisterType<UnitTestCurrentUser>()
                .AsSelf()
                .SingleInstance();

            containerBuilder.RegisterType<Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory>()
                .As<Microsoft.Extensions.Logging.ILoggerFactory>()
                .SingleInstance();

            RegisterDependency(containerBuilder);

            return containerBuilder;
        }

        /// <summary>
        /// To register something needed for tests, override this method.
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void RegisterDependency(ContainerBuilder builder)
        {
            // classes inhert from this my override this method to register something needed.
        }

        private void ConstructProperties()
        {
            //Make FakeMessagePublisherWrapper
            MessageBus = Resolve<IMessageBus>();
            lock (MessageBus)
            {
                MessageBus.RegisterMessageHandlers(_iocContainer);
            }
        }

        /// <summary>
        /// Shortcut for resolving a object from IocContainer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T Resolve<T>()
        {
            return IocResolver.Resolve<T>();
        }


        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    _iocContainer.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                _iocContainer = null;
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~AbstractionTestBase() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
