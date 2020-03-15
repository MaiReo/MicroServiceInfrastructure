using Core.Abstractions.Dependency;
using Core.Messages.Store;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Abstractions.Tests.Fakes
{
    public class FakePublishedMessageStorageProvider : IPublishedMessageStorageProvider, IMessageStorageProvider, ILifestyleSingleton
    {
        public FakePublishedMessageStorageProvider()
        {
            InMemoryStore = new ConcurrentDictionary<string, MessageModel>();
            SaveAsyncParameters = new List<MessageModel>();
        }
        public ConcurrentDictionary<string, MessageModel> InMemoryStore { get; }

        public List<MessageModel> SaveAsyncParameters { get; }

        public ValueTask<MessageModel> FindAsync(string hash, CancellationToken cancellationToken = default)
        {
            InMemoryStore.TryGetValue(hash, out var model);
            return new ValueTask<MessageModel>(model);
        }

        public ValueTask SaveAsync(MessageModel model, CancellationToken cancellationToken = default)
        {
            SaveAsyncParameters.Add(model);
            InMemoryStore.AddOrUpdate(model.Hash, model, (key, old) => model);
            return new ValueTask();
        }
    }
}
