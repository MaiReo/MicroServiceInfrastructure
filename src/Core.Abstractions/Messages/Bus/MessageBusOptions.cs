namespace Core.Messages.Bus
{
    public class MessageBusOptions : IMessageBusOptions
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        public string HostName { get; set; }

        public string ExchangeName { get; set; }

        public string QueueName { get; set; }

        public int Port { get; set; }

        public bool UseServiceDiscovery { get; set; }

        public string HostServiceName { get; set; }

        public bool? QueuePerConsumer { get; set; }
    }
}