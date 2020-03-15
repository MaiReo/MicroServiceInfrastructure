using Autofac;
using Autofac.Core;
using Core.Messages.Bus.Factories;
using Core.Messages.Bus.Internal;
using System;
using System.Linq;

namespace Core.Messages.Bus.Extensions
{
    public static class AutoFacMessageBusExtensions
    {
        public static IMessageBus RegisterMessageHandlers(this IMessageBus messageBus, ILifetimeScope lifetimeScope)
        {
            foreach (var registration in lifetimeScope.ComponentRegistry.Registrations)
            {
                var handlerType = registration.Activator.LimitType;
                var services = registration.Services.OfType<IServiceWithType>().Select(x => x.ServiceType);
                foreach (var descriptor in handlerType.GetMessageHandlerDescriptors(services))
                {
                    messageBus.Register(descriptor.MessageType, new IocMessageHandlerFactory(descriptor));
                }
            }
            return messageBus;
        }
    }
}
