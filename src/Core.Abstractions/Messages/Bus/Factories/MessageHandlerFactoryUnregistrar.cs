using Core.Messages.Bus;
using System;

namespace Core.Messages.Bus.Factories
{
    /// <summary>
    /// Used to unregister a <see cref="IMessageHandlerFactory"/> on <see cref="Dispose"/> method.
    /// </summary>
    internal class MessageHandlerFactoryUnregistrar : IDisposable
    {
       

        private readonly IMessageHandlerFactoryStore _factoryStore;
        private readonly Type _messageType;
        private readonly IMessageHandlerFactory _factory;

        public MessageHandlerFactoryUnregistrar(IMessageHandlerFactoryStore factoryStore, Type messageType, IMessageHandlerFactory factory)
        {
            this._factoryStore = factoryStore;
            this._messageType = messageType;
            this._factory = factory;
        }

        public void Dispose()
        {
            this._factoryStore.Unregister(_messageType, _factory);
        }
    }
}