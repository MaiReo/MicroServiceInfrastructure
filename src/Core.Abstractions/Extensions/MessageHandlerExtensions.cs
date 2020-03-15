using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Messages.Bus.Internal
{
    public static class MessageHandlerExtensions
    {

        /// <summary>
        /// Do not use this api in your code directly. This api may be change or removed.
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public static IEnumerable<MessageHandlerDescriptor> GetMessageHandlerDescriptors(this Type type, IEnumerable<Type> services)
        {
            if (type.IsAbstract)
            {
                yield break;
            }
            if (type.IsInterface)
            {
                yield break;
            }

            foreach (var descriptor in GetSyncMessageHandlerDescriptors(type, services))
            {
                yield return descriptor;
            }

            foreach (var descriptor in GetSyncRichMessageHandlerDescriptors(type, services))
            {
                yield return descriptor;
            }

            foreach (var descriptor in GetAsyncMessageHandlerDescriptors(type, services))
            {
                yield return descriptor;
            }

            foreach (var descriptor in GetAsyncRichMessageHandlerDescriptors(type, services))
            {
                yield return descriptor;
            }

        }

        private static IEnumerable<MessageHandlerDescriptor> GetSyncMessageHandlerDescriptors(Type type, IEnumerable<Type> services) => services
            .Where(i => i.IsInterface)
            .Concat(services.Where(i=>!i.IsInterface).SelectMany(x => x.GetInterfaces()))
            .Where(i => i.IsGenericType)
            .Where(i => i.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
            .Distinct()
            .Select(i => new MessageHandlerDescriptor(type, i.GetGenericArguments()[0]));

        private static IEnumerable<MessageHandlerDescriptor> GetSyncRichMessageHandlerDescriptors(Type type, IEnumerable<Type> services) => services
            .Where(i => i.IsInterface)
            .Concat(services.Where(i => !i.IsInterface).SelectMany(x => x.GetInterfaces()))
            .Where(i => i.IsGenericType)
            .Where(i => i.GetGenericTypeDefinition() == typeof(IRichMessageHandler<>))
            .Distinct()
            .Select(i => new MessageHandlerDescriptor(type, i.GetGenericArguments()[0], isRich: true));


        private static IEnumerable<MessageHandlerDescriptor> GetAsyncMessageHandlerDescriptors(Type type, IEnumerable<Type> services) => services
            .Where(i => i.IsInterface)
            .Concat(services.Where(i => !i.IsInterface).SelectMany(x => x.GetInterfaces()))
            .Where(i => i.IsGenericType)
            .Where(i => i.GetGenericTypeDefinition() == typeof(IAsyncMessageHandler<>))
            .Distinct()
            .Select(i => new MessageHandlerDescriptor(type, i.GetGenericArguments()[0], isAsync: true));


        private static IEnumerable<MessageHandlerDescriptor> GetAsyncRichMessageHandlerDescriptors(Type type, IEnumerable<Type> services) => services
            .Where(i => i.IsInterface)
            .Concat(services.Where(i => !i.IsInterface).SelectMany(x => x.GetInterfaces()))
            .Where(i => i.IsGenericType)
            .Where(i => i.GetGenericTypeDefinition() == typeof(IAsyncRichMessageHandler<>))
            .Distinct()
            .Select(i => new MessageHandlerDescriptor(type, i.GetGenericArguments()[0], true, true));
    }
}
