using Autofac;
using Core.Messages.Bus;
using Core.Messages.Bus.Extensions;
using Core.PersistentStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder
{
    public static class IntegrationApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMessageBus(this IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
            using var lifetimeScope = app.ApplicationServices.GetRequiredService<ILifetimeScope>();
            var messageBus = lifetimeScope.Resolve<IMessageBus>();
            messageBus.RegisterMessageHandlers(lifetimeScope);

            app.UseRabbitMQWithAutoSubscribe(applicationLifetime.ApplicationStopping);
            return app;
        }
    }
}
