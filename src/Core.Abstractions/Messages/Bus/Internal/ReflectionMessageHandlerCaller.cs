using Core.Messages.Bus.Factories;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Bus.Internal
{
    public class ReflectionMessageHandlerCaller : IMessageHandlerCaller
    {
        public static IMessageHandlerCaller Instance => new ReflectionMessageHandlerCaller();

        public async ValueTask<bool> CallAsync(
            IMessageScope scope, IMessageHandlerFactory handlerFactory,
            IMessage message, IRichMessageDescriptor messageDescriptor,
            List<Exception> exceptions = null,
            CancellationToken cancellationToken = default)
        {
            exceptions = exceptions ?? new List<Exception>();
            var handlerDescriptor = handlerFactory.GetHandlerDescriptor();
            if (handlerDescriptor.IsAsync)
            {
                if (handlerDescriptor.IsRich)
                {
                    await ProcessRichMessageHandlingExceptionAsync(scope, handlerFactory, handlerDescriptor.MessageType, message, messageDescriptor, exceptions);
                }
                else
                {
                    await ProcessMessageHandlingExceptionAsync(scope, handlerFactory, handlerDescriptor.MessageType, message, exceptions);
                }
            }
            else
            {
                if (handlerDescriptor.IsRich)
                {
                    ProcessRichMessageHandlingException(scope, handlerFactory, handlerDescriptor.MessageType, message, messageDescriptor, exceptions);
                }
                else
                {
                    ProcessMessageHandlingException(scope, handlerFactory, handlerDescriptor.MessageType, message, exceptions);
                }

            }

            return true;
        }


        private async ValueTask ProcessRichMessageHandlingExceptionAsync(IMessageScope scope, IMessageHandlerFactory asyncHandlerFactory, Type messageType, IMessage message, IRichMessageDescriptor descriptor, List<Exception> exceptions)
        {
            var handlerInstance = asyncHandlerFactory.GetHandler(scope);

            try
            {
                if (handlerInstance == null)
                {
                    throw new ArgumentNullException($"Registered async rich message handler for message type {messageType.Name} is null!");
                }

                var ifType = typeof(IAsyncRichMessageHandler<>).MakeGenericType(messageType);

                var method = ifType.GetMethod(
                    nameof(IAsyncRichMessageHandler<IMessage>.HandleMessageAsync),
                    new[] { messageType, typeof(IRichMessageDescriptor) }
                );



                await (ValueTask)method.Invoke(handlerInstance, new object[] { message, descriptor });
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                asyncHandlerFactory.ReleaseHandler(scope, handlerInstance);
            }
        }

        private async ValueTask ProcessMessageHandlingExceptionAsync(IMessageScope scope, IMessageHandlerFactory asyncHandlerFactory, Type messageType, IMessage message, List<Exception> exceptions)
        {
            var asyncEventHandler = asyncHandlerFactory.GetHandler(scope);

            try
            {
                if (asyncEventHandler == null)
                {
                    throw new ArgumentNullException($"Registered async message handler for message type {messageType.Name} is null!");
                }

                var asyncHandlerType = typeof(IAsyncMessageHandler<>).MakeGenericType(messageType);

                var method = asyncHandlerType.GetMethod(
                    nameof(IAsyncMessageHandler<IMessage>.HandleMessageAsync),
                    new[] { messageType }
                );

                await (ValueTask)method.Invoke(asyncEventHandler, new object[] { message });
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                asyncHandlerFactory.ReleaseHandler(scope, asyncEventHandler);
            }
        }

        private void ProcessRichMessageHandlingException(IMessageScope scope, IMessageHandlerFactory handlerFactory, Type messageType, IMessage message, IRichMessageDescriptor descriptor, List<Exception> exceptions)
        {
            var eventHandler = handlerFactory.GetHandler(scope);
            try
            {
                if (eventHandler == null)
                {
                    throw new ArgumentNullException($"Registered rich message handler for message type {messageType.Name} is null!");
                }

                var handlerType = typeof(IRichMessageHandler<>).MakeGenericType(messageType);

                var method = handlerType.GetMethod(
                    nameof(IRichMessageHandler<IMessage>.HandleMessage),
                    new[] { messageType, typeof(IRichMessageDescriptor) }
                );

                method.Invoke(eventHandler, new object[] { message, descriptor });
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                handlerFactory.ReleaseHandler(scope, eventHandler);
            }
        }

        private void ProcessMessageHandlingException(IMessageScope scope, IMessageHandlerFactory handlerFactory, Type messageType, IMessage message, List<Exception> exceptions)
        {
            var eventHandler = handlerFactory.GetHandler(scope);
            try
            {
                if (eventHandler == null)
                {
                    throw new ArgumentNullException($"Registered message handler for message type {messageType.Name} is null!");
                }

                var handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType);

                var method = handlerType.GetMethod(
                    nameof(IMessageHandler<IMessage>.HandleMessage),
                    new[] { messageType }
                );

                method.Invoke(eventHandler, new object[] { message });
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                handlerFactory.ReleaseHandler(scope, eventHandler);
            }
        }
    }
}
