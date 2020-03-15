using Core.Abstractions.Dependency;
using Core.PersistentStore.Repositories;
using System;
using System.Linq;
using System.Reflection;

namespace Autofac
{

    /// <summary>
    /// 按约定注册服务
    /// </summary>
    public static class ContractConventionDependencyRegistrar
    {
        /// <summary>
        /// 从包含<typeparamref name="T"/>类型的程序集扫描服务注册到容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        public static void RegisterAssemblyByConvention(this ContainerBuilder builder, Assembly assembly)
        {


            builder.RegisterAssemblyTypes(assembly)
                   .AssignableTo<ILifestyleSingleton>()
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .PropertiesAutowired()
                   .SingleInstance();

            builder.RegisterAssemblyTypes(assembly)
                   .AssignableTo<ILifestyleScoped>()
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .PropertiesAutowired()
                   .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(assembly)
                   .AssignableTo<ILifestyleTransient>()
                   .AsImplementedInterfaces()
                   .AsSelf()
                   .PropertiesAutowired()
                   .InstancePerDependency();

            builder.RegisterAssemblyTypes(assembly)
                  .AssignableTo<ILifestyleSingletonSelf>()
                  .AsSelf()
                  .PropertiesAutowired()
                  .SingleInstance();

            builder.RegisterAssemblyTypes(assembly)
                   .AssignableTo<ILifestyleTransientSelf>()
                   .AsSelf()
                   .PropertiesAutowired()
                   .InstancePerDependency();


            RegisterRepository(builder, assembly, typeof(IRepository<>));
            RegisterRepository(builder, assembly, typeof(IRepository<,>));
            RegisterRepository(builder, assembly, typeof(IAsyncRepository<>));
            RegisterRepository(builder, assembly, typeof(IAsyncRepository<,>));

            //Don't work.
            //builder.RegisterAssemblyTypes(assembly)
            //    .AsClosedTypesOf(typeof(IRepository<>))
            //    .AsClosedTypesOf(typeof(IRepository<,>))
            //    .AsClosedTypesOf(typeof(IAsyncRepository<>))
            //    .AsClosedTypesOf(typeof(IAsyncRepository<,>))
            //    .InstancePerDependency();
        }

        private static void RegisterRepository(ContainerBuilder builder, Assembly assembly, Type openGenericType)
        {
            var repositoryTypes = assembly.DefinedTypes
               .Where(t => t.IsAbstract == false)
               .Where(t => t.IsInterface == false)
               .Where(t => t.GetTypeInfo().ImplementedInterfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == openGenericType))
               .ToList();

            foreach (var repositoryType in repositoryTypes)
            {
                builder.RegisterGeneric(repositoryType).As(openGenericType).InstancePerDependency().PropertiesAutowired();
            }
        }
    }
}
