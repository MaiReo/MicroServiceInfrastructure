using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Core.Abstractions
{
    internal static class AutoFacContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterIfNot<TImplementation>
            (this ContainerBuilder builder,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where TImplementation : class
        {
            builder.RegisterType<TImplementation>()
                .AsSelf()
                .PropertiesAutowired()
                .IfNotRegistered(typeof(TImplementation))
                .If(lifeStyle == ServiceLifetime.Singleton, x => x.SingleInstance().ExternallyOwned())
                .If(lifeStyle == ServiceLifetime.Transient, x => x.InstancePerDependency());

            return builder;
        }

        public static ContainerBuilder RegisterIfNot<TService, TImplementation>
            (this ContainerBuilder builder,
            ServiceLifetime lifeStyle = ServiceLifetime.Singleton)
            where TImplementation : class, TService
            where TService : class
        {
            builder.RegisterType<TImplementation>()
                .AsSelf()
                .As<TService>()
                .PropertiesAutowired()
                .IfNotRegistered(typeof(TService))
                .If(lifeStyle == ServiceLifetime.Singleton, x => x.SingleInstance().ExternallyOwned())
                .If(lifeStyle == ServiceLifetime.Transient, x => x.InstancePerDependency());

            return builder;
        }

        public static ContainerBuilder RegisterInstanceIfNot<TService>
            (this ContainerBuilder builder, TService instance)
            where TService : class
        {
            builder.RegisterInstance(instance)
                .AsSelf()
                .If(instance.GetType() != typeof(TService), x => x.As<TService>())
                .PropertiesAutowired()
                .IfNotRegistered(typeof(TService))
                .SingleInstance()
                .ExternallyOwned();
            return builder;
        }

        private static T If<T>(this T @this, bool condition, Action<T> @true)
        {
            if (condition)
            {
                @true?.Invoke(@this);
            }
            return @this;
        }
    }
}
