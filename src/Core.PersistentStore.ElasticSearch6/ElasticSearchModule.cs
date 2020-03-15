using Autofac;
using Core.PersistentStore.Repositories;

namespace Core.PersistentStore
{
    public class ElasticSearchModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ElasticSearchRepository<>))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        }
    }
}
