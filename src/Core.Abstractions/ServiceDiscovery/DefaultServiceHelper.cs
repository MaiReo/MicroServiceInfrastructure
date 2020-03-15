using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.ServiceDiscovery
{
    public class DefaultServiceHelper : IServiceHelper
    {
        public DefaultServiceHelper(ServiceDiscoveryConfiguration configuration)
        {
            Configuration = configuration;
        }

        public ServiceDiscoveryConfiguration Configuration { get; }

        protected virtual string ServiceId => Assembly.GetEntryAssembly().GetName().Name;

        protected virtual string ServiceName => ServiceId;

        protected virtual IEnumerable<string> ServiceTags => new[] { "core" };

        protected virtual string Normalize(in string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Guid.NewGuid().ToString("N");
            }
            var newName = name.ToLowerInvariant().Replace('.', '-');
            return newName;
        }

        string IServiceHelper.GetRunningServiceId() => string.IsNullOrWhiteSpace(Configuration.ServiceId) ? Normalize(ServiceId) : Configuration.ServiceId;

        string IServiceHelper.GetRunningServiceName() => string.IsNullOrWhiteSpace(Configuration.ServiceName) ? Normalize(ServiceName) : Configuration.ServiceName;

        IEnumerable<string> IServiceHelper.GetRunningServiceTags() => Configuration.ServiceTags ?? ServiceTags;
    }
}
