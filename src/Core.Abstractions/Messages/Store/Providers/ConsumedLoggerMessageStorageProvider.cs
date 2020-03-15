using Microsoft.Extensions.Logging;

namespace Core.Messages.Store
{
    public class ConsumedLoggerMessageStorageProvider : LoggerMessageStorageProvider, IConsumedMessageStorageProvider, IMessageStorageProvider
    {
        public ConsumedLoggerMessageStorageProvider(
            ILogger<ConsumedLoggerMessageStorageProvider> logger = null) : base(logger)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string LogPrefix => "[Consumed]";
    }
}
