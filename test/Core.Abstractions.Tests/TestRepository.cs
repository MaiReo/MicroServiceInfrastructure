using Core.PersistentStore;
using Core.PersistentStore.Repositories;

namespace Core.Abstractions.Tests
{
    internal class TestRepository<TEntity> : EFAsyncRepository<TestDbContext, TEntity> where TEntity : class, IEntity
    {
        public TestRepository(IDbContextResolver<TestDbContext> dbContextResolver) : base(dbContextResolver)
        {
        }
    }
    internal class TestRepository<TEntity, TKey> : EFAsyncRepository<TestDbContext, TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        public TestRepository(IDbContextResolver<TestDbContext> dbContextResolver) : base(dbContextResolver)
        {
        }
    }
}
