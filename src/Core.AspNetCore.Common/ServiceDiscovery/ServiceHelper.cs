using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace Core.ServiceDiscovery
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceHelper : DefaultServiceHelper, IServiceHelper
    {
        private readonly IHostEnvironment hostEnvironment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public ServiceHelper(IHostEnvironment hostEnvironment, ServiceDiscoveryConfiguration configuration) : base(configuration)
        {
            this.hostEnvironment = hostEnvironment;
        }

        protected override string ServiceName => hostEnvironment.ApplicationName;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> ServiceTags => Enumerable.Empty<string>();
    }
}
