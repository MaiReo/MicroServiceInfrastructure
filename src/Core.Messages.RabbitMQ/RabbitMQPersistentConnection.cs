using Core.ServiceDiscovery;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;

namespace Core.Messages
{
    public class RabbitMQPersistentConnection : IRabbitMQPersistentConnection, IDisposable
    {
        private readonly IConnectionFactoryResolver _connectionFactoryResolver;
        private readonly ServiceDiscoveryConfiguration _serviceDiscoveryConfiguration;
        private readonly ILogger _logger;
        private IConnection _connection;
        private bool _disposed;

        private static readonly object sync_root = new object();

        public RabbitMQPersistentConnection(
            IConnectionFactoryResolver connectionFactoryResolver,
            ServiceDiscoveryConfiguration serviceDiscoveryConfiguration,
            ILogger<RabbitMQPersistentConnection> logger)
        {
            _connectionFactoryResolver = connectionFactoryResolver ?? throw new ArgumentNullException(nameof(connectionFactoryResolver));
            _serviceDiscoveryConfiguration = serviceDiscoveryConfiguration;
            _logger = (ILogger)logger ?? NullLogger.Instance;
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        public IModel CreateModel()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(_connection));
            }
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            if (_connection == null)
            {
                return;
            }
            try
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
            catch (IOException ex)
            {
                _logger.LogCritical("无法关闭到RabbitMQ的连接", ex);
            }
        }

        public bool TryConnect()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(_connection));
            }
            if (IsConnected)
            {
                return true;
            }
            _logger.LogInformation("RabbitMQ Client is trying to connect");
            lock (sync_root)
            {
                _connection = _connectionFactoryResolver.Resolve().CreateConnection(_serviceDiscoveryConfiguration.ServiceName);

                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.ConnectionBlocked += OnConnectionBlocked;
                _connection.RecoverySucceeded += OnConnectionRecoverySucceeded;

                _logger.LogInformation($"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");

                return true;
            }
        }

        private void OnConnectionRecoverySucceeded(object sender, EventArgs e)
        {
            _logger.LogWarning($"A RabbitMQ connection recovery succeeded");
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            var connection = sender as IConnection;
            _logger.LogWarning($"A RabbitMQ connection is blocked. Reason: {e.Reason}.");
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs eventArgs)
        {
            var connection = sender as IConnection;
            switch (eventArgs.Initiator)
            {
                case ShutdownInitiator.Application:
                    if (eventArgs.ReplyCode == 200)
                    {
                        connection.ConnectionShutdown -= OnConnectionShutdown;
                        connection.ConnectionBlocked -= OnConnectionBlocked;
                        _logger.LogInformation($"A RabbitMQ connection is shutdown by app.");
                        return;
                    }
                    break;
                case ShutdownInitiator.Library:
                    if (eventArgs.ReplyCode == 541) //Unhandled exception
                    {
                        _logger.LogWarning($"A RabbitMQ connection is shutdown, cause by {(eventArgs.Cause as Exception).Message}");
                    }
                    break;
                case ShutdownInitiator.Peer:
                    break;
                default:
                    break;
            }
            
        }
    }
}