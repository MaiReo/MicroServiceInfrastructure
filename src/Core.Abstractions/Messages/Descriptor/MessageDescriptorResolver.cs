using Core.Messages.Bus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Messages
{
    public class MessageDescriptorResolver : IMessageDescriptorResolver
    {
        private readonly IMessageBusOptions messageBusOptions;

        public MessageDescriptorResolver(IMessageBusOptions messageBusOptions)
        {
            this.messageBusOptions = messageBusOptions;
        }

        public IMessageDescriptor Resolve<T>(T message) where T : IMessage
        {
            var type = message?.GetType() ?? typeof(T);
            return Resolve(type);
        }

        public IMessageDescriptor Resolve(Type messageType)
        {
            var descriptor = messageType
                .GetCustomAttributes(false)
                .OfType<IMessageDescriptorProvider>()
                .FirstOrDefault()
                ?.GetMessageDescriptor(messageBusOptions.ExchangeName, messageType.Name)
                ?? new MessageDescriptor(string.Empty, messageType.Name);

            return descriptor;
        }

        public Type Resolve(IMessageDescriptor descriptor, IEnumerable<Type> types)
        {
            if (descriptor == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(descriptor.MessageTopic))
            {
                return null;
            }
            if (types == null)
            {
                return null;
            }

            var messageType = types.Select(type => new
            {
                Type = type,
                Descriptor = Resolve(type)
            })
            .FirstOrDefault(x => MessageDescriptorEqualityComparer.Instance.Equals(x.Descriptor, descriptor))
            ?.Type;

            return messageType;
        }
    }
}