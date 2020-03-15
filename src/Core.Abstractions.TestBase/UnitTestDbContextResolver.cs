using Autofac;
using Core.PersistentStore;
using Core.PersistentStore.Uow;
using Core.Session;
using Microsoft.EntityFrameworkCore;

namespace Core.TestBase
{
    public class UnitTestDbContextResolver<TDbContext> : IDbContextResolver<TDbContext> where TDbContext : DbContext
    {
        private readonly ILifetimeScope _lifetimeScope;

        public UnitTestDbContextResolver(ILifetimeScope lifetimeScope)
        {
            this._lifetimeScope = lifetimeScope;
        }

        public TDbContext GetDbContext()
        {
            var dbContext = _lifetimeScope.Resolve<TDbContext>();
            if (dbContext is ICoreSessionProviderRequired sessionProviderRequired)
            {
                sessionProviderRequired.SessionProvider = _lifetimeScope.Resolve<UnitTestCoreSessionProvider>();
            }
            if (dbContext is ICurrentUnitOfWorkRequired unitOfWorkRequired)
            {
                unitOfWorkRequired.CurrentUnitOfWork = _lifetimeScope.Resolve<ICurrentUnitOfWork>();
            }
            return dbContext;
        }
    }
}
