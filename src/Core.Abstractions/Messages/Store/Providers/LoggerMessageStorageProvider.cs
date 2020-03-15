using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Store
{
    public abstract class LoggerMessageStorageProvider : IMessageStorageProvider
    {
        public LoggerMessageStorageProvider(ILogger logger = default)
        {
            _logger = logger ?? NullLogger.Instance;
        }
        private readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        protected abstract string LogPrefix { get; }


        public ValueTask<MessageModel> FindAsync(string hash, CancellationToken cancellationToken = default)
        {
            return new ValueTask<MessageModel>(default(MessageModel));
        }

        public ValueTask SaveAsync(MessageModel model, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"{LogPrefix}[{model.Group}][{model.Topic}][{model.Hash}]{model.Message}");
            return new ValueTask();
        }
    }
}
