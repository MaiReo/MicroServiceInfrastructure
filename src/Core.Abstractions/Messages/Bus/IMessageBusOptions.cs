namespace Core.Messages.Bus
{
    public interface IMessageBusOptions
    {
        string UserName { get; }

        string Password { get; }

        string VirtualHost { get; }

        string HostName { get; }

        string ExchangeName { get; }

        string QueueName { get; }

        int Port { get; }

        bool UseServiceDiscovery { get; }

        string HostServiceName { get; }

        bool? QueuePerConsumer { get; }
    }
}