using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Messages.Bus.Factories;

namespace Core.Messages.Bus.Internal
{
    public class ExpressionTreeMessageHandlerCaller : IMessageHandlerCaller
    {
        private readonly ConcurrentDictionary<Type, Func<IMessageHandler, IMessage, IRichMessageDescriptor, ValueTask>> _asyncHandlerCallCache;
        private readonly ConcurrentDictionary<Type, Action<IMessageHandler, IMessage, IRichMessageDescriptor>> _syncHandlerCallCache;

        public ExpressionTreeMessageHandlerCaller()
        {
            _asyncHandlerCallCache = new ConcurrentDictionary<Type, Func<IMessageHandler, IMessage, IRichMessageDescriptor, ValueTask>>();
            _syncHandlerCallCache = new ConcurrentDictionary<Type, Action<IMessageHandler, IMessage, IRichMessageDescriptor>>();
        }

        private Func<IMessageHandler, IMessage, IRichMessageDescriptor, ValueTask> GetOrCreateAsyncHandlerCallCache(MessageHandlerDescriptor handlerDescriptor)
        {
            Func<IMessageHandler, IMessage, IRichMessageDescriptor, ValueTask> @delegate = null;

            if (!handlerDescriptor.IsAsync)
            {
                return @delegate;
            }

            var handlerIfaceType = handlerDescriptor.IsRich ? typeof(IAsyncRichMessageHandler<>).MakeGenericType(handlerDescriptor.MessageType)
                                          : typeof(IAsyncMessageHandler<>).MakeGenericType(handlerDescriptor.MessageType);

            @delegate = _asyncHandlerCallCache.GetOrAdd(handlerIfaceType, (ifaceType) =>
            {
                const string methodName = nameof(IAsyncMessageHandler<IMessage>.HandleMessageAsync);

                ParameterExpression instanceParameter = Expression.Parameter(typeof(IMessageHandler), "@handler");
                ParameterExpression messageParameter = Expression.Parameter(typeof(IMessage), "message");
                ParameterExpression descriptorParameter = Expression.Parameter(typeof(IRichMessageDescriptor), "descriptor");

                Expression convertInstanceToType = Expression.Convert(instanceParameter, handlerIfaceType);
                Expression convertMessageToType = Expression.Convert(messageParameter, handlerDescriptor.MessageType);

                Expression call = handlerDescriptor.IsRich ? Expression.Call(convertInstanceToType, methodName, null, convertMessageToType, descriptorParameter)
                                         : Expression.Call(convertInstanceToType, methodName, null, convertMessageToType);

                var lambda = Expression.Lambda<Func<IMessageHandler, IMessage, IRichMessageDescriptor, ValueTask>>(call, instanceParameter, messageParameter, descriptorParameter);

                return lambda.Compile();

            });
            return @delegate;
        }

        private Action<IMessageHandler, IMessage, IRichMessageDescriptor> GetOrCreateSyncHandlerCallCache(MessageHandlerDescriptor handlerDescriptor)
        {
            Action<IMessageHandler, IMessage, IRichMessageDescriptor> @delegate = null;

            if (handlerDescriptor.IsAsync)
            {
                return @delegate;
            }

            var handlerIfaceType = handlerDescriptor.IsRich ? typeof(IRichMessageHandler<>).MakeGenericType(handlerDescriptor.MessageType)
                                          : typeof(IMessageHandler<>).MakeGenericType(handlerDescriptor.MessageType);

            @delegate = _syncHandlerCallCache.GetOrAdd(handlerIfaceType, (ifaceType) =>
            {
                const string methodName = nameof(IMessageHandler<IMessage>.HandleMessage);

                ParameterExpression instanceParameter = Expression.Parameter(typeof(IMessageHandler), "@handler");
                ParameterExpression messageParameter = Expression.Parameter(typeof(IMessage), "message");
                ParameterExpression descriptorParameter = Expression.Parameter(typeof(IRichMessageDescriptor), "descriptor");

                Expression convertInstanceToType = Expression.Convert(instanceParameter, handlerIfaceType);
                Expression convertMessageToType = Expression.Convert(messageParameter, handlerDescriptor.MessageType);

                Expression call = handlerDescriptor.IsRich ? Expression.Call(convertInstanceToType, methodName, null, convertMessageToType, descriptorParameter)
                                         : Expression.Call(convertInstanceToType, methodName, null, convertMessageToType);

                var lambda = Expression.Lambda<Action<IMessageHandler, IMessage, IRichMessageDescriptor>>(call, instanceParameter, messageParameter, descriptorParameter);

                return lambda.Compile();

            });
            return @delegate;
        }

        public async ValueTask<bool> CallAsync(IMessageScope scope, IMessageHandlerFactory handlerFactory, IMessage message, IRichMessageDescriptor messageDescriptor, List<Exception> exceptions = null, CancellationToken cancellationToken = default)
        {
            var handlerDescriptor = handlerFactory.GetHandlerDescriptor();
            IMessageHandler handler = null;
            try
            {
                handler = handlerFactory.GetHandler(scope);

                if (handlerDescriptor.IsAsync)
                {
                    await GetOrCreateAsyncHandlerCallCache(handlerDescriptor).Invoke(handler, message, messageDescriptor);
                }
                else
                {
                    GetOrCreateSyncHandlerCallCache(handlerDescriptor).Invoke(handler, message, messageDescriptor);
                }

            }
            catch (TargetInvocationException e)
            {
                exceptions?.Add(e.InnerException);
                return false;
            }
            catch (Exception e)
            {
                exceptions?.Add(e);
                return false;
            }
            finally
            {
                if (handler != null)
                {
                    handlerFactory.ReleaseHandler(scope, handler);
                }
            }
            return true;
        }


    }
}
