using Autofac;
using MediatR;
using MediatR.Pipeline;

namespace Core.Events.Bus
{
    public class EventBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<MediatorModule>();

            builder.RegisterType<EventBus>()
                .As<IEventBus>()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();
        }
    }

    internal sealed class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Uncomment to enable polymorphic dispatching of requests, but note that
            // this will conflict with generic pipeline behaviors
            // builder.RegisterSource(new ContravariantRegistrationSource());

            // Mediator itself
            builder
                .RegisterType<MediatR.Mediator>()
                .As<MediatR.IMediator>()
                .InstancePerLifetimeScope();

            // request & notification handlers
            builder.Register<MediatR.ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });


            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>))
                .As(typeof(IPipelineBehavior<,>));

            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>))
                .As(typeof(IPipelineBehavior<,>));

            builder.RegisterGeneric(typeof(RequestExceptionActionProcessorBehavior<,>))
                .As(typeof(IPipelineBehavior<,>));

            builder.RegisterGeneric(typeof(RequestExceptionProcessorBehavior<,>))
                .As(typeof(IPipelineBehavior<,>));
        }
    }
}

