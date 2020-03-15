using Autofac;
using Core.PersistentStore.Repositories;
using Core.TestBase;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.Abstractions.Tests
{
    public class RepositoryTests : AbstractionTestBase<RepositoryTests, TestDbContext>
    {
        private readonly IAsyncRepository<TestEntityOne> _testEntityOneRepository;
        private readonly IAsyncRepository<TestEntityTwo> _testEntityTwoRepository;

        public RepositoryTests()
        {
            this._testEntityOneRepository = Resolve<IAsyncRepository<TestEntityOne>>();
            this._testEntityTwoRepository = Resolve<IAsyncRepository<TestEntityTwo>>();
        }

        protected override void RegisterDependency(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(TestRepository<>))
                .As(typeof(IAsyncRepository<>))
                .As(typeof(IRepository<>))
                .PropertiesAutowired()
                .InstancePerDependency();
            builder.RegisterGeneric(typeof(TestRepository<,>))
                .As(typeof(IAsyncRepository<,>))
                .As(typeof(IRepository<,>))
                .PropertiesAutowired()
                .InstancePerDependency();
        }

    }

}
