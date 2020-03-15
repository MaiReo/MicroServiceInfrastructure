using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.PersistentStore
{
    public class EntityFrameworkCoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(SimpleDbContextResolver<>))
                .AsSelf()
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .IfNotRegistered(typeof(IDbContextResolver<>))
                .InstancePerDependency();
        }
    }
}
