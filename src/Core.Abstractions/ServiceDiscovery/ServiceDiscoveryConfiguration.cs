namespace Core.ServiceDiscovery
{
    public class ServiceDiscoveryConfiguration
    {
        public const string DEFAULT_ADDRESS = "http://localhost:8500";
        public ServiceDiscoveryConfiguration()
        {
            Address = DEFAULT_ADDRESS;
        }
        public string Address { get; set; }

        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string[] ServiceTags { get; set; }

        public bool AutoRegister { get; set; }
       
    }
}
