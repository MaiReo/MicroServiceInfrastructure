using Core.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.AspNetCore.Builder
{
    public static class RabbitMQApplicationBuilderExtensions
    {
        private static IRabbitMQPersistentConnection _connection;
        private static IMessageSubscriber _messageSubscriber;
        /// <summary>
        /// Use RabbitMQ.
        /// Pass IHostApplicationLifetime.ApplicationStopping to this method.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRabbitMQWithAutoSubscribe(this IApplicationBuilder app,
            CancellationToken applicationStopping)
        {
            _connection = _connection ?? app.ApplicationServices.GetRequiredService<IRabbitMQPersistentConnection>();
            _connection.TryConnect();
            _messageSubscriber = _messageSubscriber ?? app.ApplicationServices.GetRequiredService<IMessageSubscriber>();
            _messageSubscriber.AutoSubscribe();
            applicationStopping.Register(_connection.Dispose);
            return app;
        }
    }
}
