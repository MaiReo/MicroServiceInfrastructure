using Core.PersistentStore.Uow;
using Core.Session;
using Microsoft.EntityFrameworkCore;

namespace Core.PersistentStore
{
    public class SimpleDbContextResolver<TDbContext> : IDbContextResolver<TDbContext> where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private bool _sessionProviderSetted;
        private bool _uowSetted;

        public SimpleDbContextResolver(TDbContext dbContext) => this._dbContext = dbContext;

        public virtual ICoreSessionProvider SessionProvider { get; set; }
        public virtual ICurrentUnitOfWork CurrentUnitOfWork { get; set; }


        public TDbContext GetDbContext()
        {
            if (!_sessionProviderSetted)
            {
                if (SessionProvider != null && _dbContext is ICoreSessionProviderRequired sessionProviderRequired)
                {
                    sessionProviderRequired.SessionProvider = SessionProvider;
                    _sessionProviderSetted = true;
                }
            }

            if (!_uowSetted)
            {
                if (CurrentUnitOfWork != null && _dbContext is ICurrentUnitOfWorkRequired currentUnitOfWorkRequired)
                {
                    currentUnitOfWorkRequired.CurrentUnitOfWork = CurrentUnitOfWork;
                    _uowSetted = true;
                }
            }
            return _dbContext;
        }
    }
}
