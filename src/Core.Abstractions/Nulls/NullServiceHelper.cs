using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.ServiceDiscovery
{
    public class NullServiceHelper : IServiceHelper
    {
        public string GetRunningServiceId()
        {
            return default;
        }

        public string GetRunningServiceName()
        {
            return default;
        }

        public IEnumerable<string> GetRunningServiceTags()
        {
            return Enumerable.Empty<string>();
        }

        public static NullServiceHelper Instance => new NullServiceHelper();
    }
}
