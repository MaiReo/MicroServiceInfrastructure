using Core.Messages.Bus;
using Core.ServiceDiscovery;
using RabbitMQ.Client;
using System;

namespace Core.Messages
{
    public class ConnectionFactoryResolver : IConnectionFactoryResolver, IDisposable
    {
        private readonly IMessageBusOptions _messageBusOptions;
        private readonly IServiceDiscoveryHelper _serviceDiscoveryHelper;
        private readonly Lazy<IConnectionFactory> _connectionFactory_singleton_lazy;

        private readonly static object sync_root = new object();
        private bool _disposed;

        public ConnectionFactoryResolver(
            IMessageBusOptions messageBusOptions,
            IServiceDiscoveryHelper serviceDiscoveryHelper)
        {
            _messageBusOptions = messageBusOptions;
            _serviceDiscoveryHelper = serviceDiscoveryHelper;
            _connectionFactory_singleton_lazy = new Lazy<IConnectionFactory>(() =>
            {
                var factory = new ConnectionFactory
                {
                    UserName = _messageBusOptions.UserName,
                    Password = _messageBusOptions.Password,
                    VirtualHost = _messageBusOptions.VirtualHost,
                    HostName = _messageBusOptions.HostName,
                    DispatchConsumersAsync = true
                };
                if (_messageBusOptions.Port > 0 && _messageBusOptions.Port < 65536)
                {
                    factory.Port = _messageBusOptions.Port;
                }
                return factory;
            }, false);
        }
        public IConnectionFactory Resolve()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(_connectionFactory_singleton_lazy));
            }
            if (!_messageBusOptions.UseServiceDiscovery || string.IsNullOrWhiteSpace(_messageBusOptions.HostServiceName))
            {
                return _connectionFactory_singleton_lazy.Value;
            }
            lock (sync_root)
            {
                var (address, port) = _serviceDiscoveryHelper.GetServiceAddress(_messageBusOptions.HostServiceName);
                return new ConnectionFactory
                {
                    UserName = _messageBusOptions.UserName,
                    Password = _messageBusOptions.Password,
                    VirtualHost = _messageBusOptions.VirtualHost,
                    HostName = address,
                    Port = port,
                    DispatchConsumersAsync = true,
                };
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
