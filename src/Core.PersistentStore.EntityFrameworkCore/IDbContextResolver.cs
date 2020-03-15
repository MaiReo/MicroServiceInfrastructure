using Microsoft.EntityFrameworkCore;

namespace Core.PersistentStore
{
    public interface IDbContextResolver<out TDbContext> where TDbContext : DbContext
    {
        TDbContext GetDbContext();
    }
}
