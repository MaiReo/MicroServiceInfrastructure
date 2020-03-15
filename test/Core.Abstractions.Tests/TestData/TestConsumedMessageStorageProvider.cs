using Core.Messages.Store;
using Core.PersistentStore.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Abstractions.Tests
{
    public class TestConsumedMessageStorageProvider : IConsumedMessageStorageProvider, IMessageStorageProvider
    {
        private readonly IAsyncRepository<TestConsumedMessageLog, Guid> repository;

        public TestConsumedMessageStorageProvider(IAsyncRepository<TestConsumedMessageLog, Guid> repository)
        {
            this.repository = repository;
        }

        public async ValueTask<MessageModel> FindAsync(string hash, CancellationToken cancellationToken = default)
        {
            var entity = await repository.FirstOrDefaultAsync(x => x.Hash == hash, cancellationToken);
            return new MessageModel(entity?.TypeName, entity?.Message, entity?.Hash, entity?.Group, entity?.Topic);
        }

        public async ValueTask SaveAsync(MessageModel model, CancellationToken cancellationToken = default)
        {
            var entity = new TestConsumedMessageLog
            {
                Hash = model.Hash,
                Group = model.Group,
                Topic = model.Topic,
                Message = model.Message,
                TypeName = model.TypeName
            };
            await repository.InsertAsync(entity, cancellationToken);
        }
    }
}
