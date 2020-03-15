using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messages
{
    public interface IConnectionFactoryResolver
    {
        IConnectionFactory Resolve();
    }
}
