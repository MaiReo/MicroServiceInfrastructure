using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Store
{
    public class PublishedLoggerMessageStorageProvider : LoggerMessageStorageProvider, IPublishedMessageStorageProvider, IMessageStorageProvider
    {
        public PublishedLoggerMessageStorageProvider(
            ILogger<PublishedLoggerMessageStorageProvider> logger = null) : base(logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string LogPrefix => "[Published]";
    }
}
