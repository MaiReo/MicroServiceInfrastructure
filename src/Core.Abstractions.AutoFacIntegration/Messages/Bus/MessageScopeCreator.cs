using Autofac;
using Core.Session;
using Core.Session.Providers;
using System;

namespace Core.Messages.Bus
{
    public class MessageScopeCreator : IMessageScopeCreator
    {
        private readonly ILifetimeScope _lifetimeScope;

        public MessageScopeCreator(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }
        public IMessageScope CreateScope(IMessage message, IRichMessageDescriptor messageDescriptor)
        {
            var sessionProvider = new MessageCoreSessionProvider(message, messageDescriptor);
            var scope = _lifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(sessionProvider)
                .As<ICoreSessionProvider>()
                .PropertiesAutowired();
            });
            var messageScope = new MessageScope(scope);
            return messageScope;
        }
    }
}
