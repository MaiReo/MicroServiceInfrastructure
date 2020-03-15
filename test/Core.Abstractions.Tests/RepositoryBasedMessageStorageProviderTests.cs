using Autofac;
using Core.Messages;
using Core.Messages.Store;
using Core.TestBase;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Core.Abstractions.Tests
{
    public class RepositoryBasedMessageStorageProviderTests : AbstractionTestBase<RepositoryBasedMessageStorageProviderTests, TestDbContext>
    {

        protected override void RegisterDependency(ContainerBuilder builder)
        {
            builder.RegisterType<TestPublishedMessageStorageProvider>()
                .As<IPublishedMessageStorageProvider>()
                .InstancePerDependency();

            builder.RegisterType<TestConsumedMessageStorageProvider>()
                .As<IConsumedMessageStorageProvider>()
                .InstancePerDependency();
        }

        [Fact(DisplayName = "EF重复消息发送测试")]
        public async Task EFDuplicateMessagePublishTest()
        {
            var testMessage = new TestMessage()
            {
                Name = "test"
            };
            for (int i = 0; i < 10; i++)
            {
                await MessageBus.PublishAsync(testMessage);
            }
            
            UsingDbContext(context => 
            {
                context.TestPublishedMessageLogs.ShouldHaveSingleItem();
            });
        }

        [Fact(DisplayName = "EF重复消息消费测试")]
        public async Task EFDuplicateMessageConsumeTest()
        {
            var testMessage = new TestMessage()
            {
                Name = "test"
            };

            var descriptor = new RichMessageDescriptor("", nameof(TestMessage));

            for (int i = 0; i < 10; i++)
            {
                await MessageBus.OnMessageReceivedAsync(testMessage, descriptor);
            }

            UsingDbContext(context =>
            {
                context.TestConsumedMessageLogs.ShouldHaveSingleItem();
            });
        }
    }
}
