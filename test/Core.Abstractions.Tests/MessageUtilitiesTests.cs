using Core.Abstractions.Tests.Fakes;
using Core.Messages;
using Core.Messages.Store;
using Core.Messages.Utilities;
using Core.TestBase;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Core.Abstractions.Tests
{
    public class MessageUtilitiesTests : AbstractionTestBase<MessageUtilitiesTests>
    {

        public MessageUtilitiesTests()
        {
            FakePublishedMessageStorageProvider = (FakePublishedMessageStorageProvider)Resolve<IPublishedMessageStorageProvider>();
            FakeConsumedMessageStorageProvider = (FakeConsumedMessageStorageProvider)Resolve<IConsumedMessageStorageProvider>();
        }

        public FakePublishedMessageStorageProvider FakePublishedMessageStorageProvider { get; }
        public FakeConsumedMessageStorageProvider FakeConsumedMessageStorageProvider { get; }

        [Fact(DisplayName = "消息哈希测试")]
        public async Task MessageHasherTest()
        {
            var hasher = Resolve<IMessageHasher>();

            var testMessage = new TestMessage()
            {
                Name = "test"
            };
            var descriptor = new RichMessageDescriptor("", "TestMessage");
            var hashes = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                var hash = await hasher.HashAsync(descriptor, testMessage);
                hashes.Add(hash);
            }
            hashes.Distinct().ShouldHaveSingleItem();

            hashes.Clear();

            for (int i = 0; i < 10; i++)
            {
                var hash = await hasher.HashAsync(descriptor, testMessage, HashAlgorithmName.MD5);
                hashes.Add(hash);
            }
            hashes.Distinct().ShouldHaveSingleItem();
            hashes.Clear();

            for (int i = 0; i < 10; i++)
            {
                var hash = await hasher.HashAsync(descriptor, testMessage, HashAlgorithmName.SHA512);
                hashes.Add(hash);
            }
            hashes.Distinct().ShouldHaveSingleItem();
            hashes.Clear();
        }

        [Fact(DisplayName = "重复消息发送测试")]
        public async Task DuplicateMessagePublishTest()
        {
            var testMessage = new TestMessage()
            {
                Name = "test"
            };

            for (int i = 0; i < 10; i++)
            {
                await MessageBus.PublishAsync(testMessage);
            }
            FakePublishedMessageStorageProvider.SaveAsyncParameters.Count.ShouldBe(1);
        }

        [Fact(DisplayName = "重复消息接收测试")]
        public async Task DuplicateMessageReceiveTest()
        {
            var testMessage = new TestMessage()
            {
                Name = "test"
            };
            var descriptor = new RichMessageDescriptor("", "TestMessage");

            for (int i = 0; i < 10; i++)
            {
                await MessageBus.OnMessageReceivedAsync(testMessage, descriptor);
            }
            FakeConsumedMessageStorageProvider.SaveAsyncParameters.Count.ShouldBe(1);
        }

        [Fact(DisplayName = "重复消息接收测试2")]
        public async Task DuplicateMessageReceiveTest2()
        {
            var testMessage = new TestMessage()
            {
                Name = "test"
            };
            var descriptor = new RichMessageDescriptor(null, "", "TestMessage", false, null, null, Guid.NewGuid().ToString(), true, null);

            for (int i = 0; i < 10; i++)
            {
                await MessageBus.OnMessageReceivedAsync(testMessage, descriptor);
            }
            FakeConsumedMessageStorageProvider.SaveAsyncParameters.Count.ShouldBe(1);
        }
        [Fact(DisplayName = "重复消息接收测试3")]
        public async Task DuplicateMessageReceiveTest3()
        {
            var testMessage = new TestMessage()
            {
                Name = "test"
            };
            var raw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testMessage));
            var descriptor = new RichMessageDescriptor(raw, "", "TestMessage", false, null, null, null, true, null);

            for (int i = 0; i < 10; i++)
            {
                await MessageBus.OnMessageReceivedAsync(testMessage, descriptor);
            }
            FakeConsumedMessageStorageProvider.SaveAsyncParameters.Count.ShouldBe(1);
        }
    }

}
