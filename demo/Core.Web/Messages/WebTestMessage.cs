using Core.Abstractions.Dependency;
using Core.Messages;
using Core.Session;
using Core.Session.Providers;
using System;

namespace Core.Web.Messages
{
    [MessageDescription(default, "debug.core.abstractions")]
    public class WebTestMessage : IMessage
    {
        public string TestMessage { get; set; }
    }

    public class WebTestMessageHandler : IRichMessageHandler<WebTestMessage>, ILifestyleTransient
    {
        private readonly ICoreSessionProvider _coreSessionProvider;

        public WebTestMessageHandler(ICoreSessionProvider coreSessionProvider)
        {
            _coreSessionProvider = coreSessionProvider;
        }

        public void HandleMessage(WebTestMessage message, IRichMessageDescriptor descriptor)
        {
            if (_coreSessionProvider is MessageCoreSessionProvider provider)
            {
                if (provider.MessageDescriptor == descriptor)
                {
                    return;
                }
            }
            throw new ApplicationException("Session错误");
        }
    }
}
