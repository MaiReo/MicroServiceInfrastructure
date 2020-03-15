using Microsoft.EntityFrameworkCore;

namespace Core.PersistentStore
{
    public interface IRepositoryWithDbContext
    {
        DbContext GetDbContext();
    }
}