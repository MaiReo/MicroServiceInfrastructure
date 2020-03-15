using Autofac;
using Core.PersistentStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.TestBase
{
    public class AbstractionTestBase<TStartup, TDbContext> : AbstractionTestBase<TStartup>
        where TStartup : class
        where TDbContext : CorePersistentStoreDbContext
    {
        protected internal override ContainerBuilder RegisterRequiredServices(IServiceCollection services)
        {
            var databaseId = Guid.NewGuid().ToString();
            services.AddDbContext<TDbContext>(
                opt =>
                {
                    opt.UseInMemoryDatabase(databaseId);
                    opt.ConfigureWarnings(warn => warn.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                },
                contextLifetime: ServiceLifetime.Transient);

            var builder = base.RegisterRequiredServices(services);

            builder.RegisterGeneric(typeof(UnitTestDbContextResolver<>))
                  .AsSelf()
                  .AsImplementedInterfaces()
                  .InstancePerDependency();
            return builder;
        }

        protected void UsingDbContext(Action<TDbContext> action)
        {
            UsingDbContext(null, action);
        }

        protected void UsingDbContext(string cityId, Action<TDbContext> action)
        {
            UsingDbContext(cityId, default, action);
        }

        protected void UsingDbContext(string cityId, Guid? companyId, Action<TDbContext> action)
        {
            using (Resolve<UnitTestCoreSessionProvider>().Use(cityId, companyId, default, default, default, default, default))
            using (var dbContext = Resolve<IDbContextResolver<TDbContext>>().GetDbContext())
            {
                action?.Invoke(dbContext);
                dbContext.SaveChanges();
            }
        }

        protected void UsingDbContext(Action<TDbContext> action,
           string cityId = default,
           Guid? companyId = default, string companyName = default,
           Guid? groupId = default, string groupName = default,
           string brokerId = default, string brokerName = default)
        {
            using (Resolve<UnitTestCoreSessionProvider>().Use(cityId, companyId, companyName, groupId, groupName, brokerId, brokerName))
            using (var dbContext = Resolve<IDbContextResolver<TDbContext>>().GetDbContext())
            {
                action?.Invoke(dbContext);
                dbContext.SaveChanges();
            }
        }

        protected T UsingDbContext<T>(Func<TDbContext, T> func,
           string cityId = default,
           Guid? companyId = default, string companyName = default,
           Guid? groupId = default, string groupName = default,
           string brokerId = default, string brokerName = default)
        {
            using (Resolve<UnitTestCoreSessionProvider>().Use(cityId, companyId, companyName, groupId, groupName, brokerId, brokerName))
            using (var dbContext = Resolve<IDbContextResolver<TDbContext>>().GetDbContext())
            {
                var returnValue = func(dbContext);
                dbContext.SaveChanges();
                return returnValue;
            }
        }

    }
}
