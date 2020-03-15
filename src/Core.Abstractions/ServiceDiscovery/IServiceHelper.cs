using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ServiceDiscovery
{
    public interface IServiceHelper
    {
        string GetRunningServiceId();
        string GetRunningServiceName();
        IEnumerable<string> GetRunningServiceTags();
    }
}
