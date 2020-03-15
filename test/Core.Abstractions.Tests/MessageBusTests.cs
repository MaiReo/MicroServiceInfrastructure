using Core.Abstractions.Dependency;
using Core.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Core.Abstractions.Tests
{

    public class TestMessage : IMessage
    {
        public string Name { get; set; }
    }

    public class TestMessage3 : IMessage
    {
        public string Name { get; set; }
    }

    public class TestMessage4 : IMessage
    {
        public string Name { get; set; }
    }

    public class TestMessageMultipleHandler : IAsyncMessageHandler<TestMessage3>, IAsyncMessageHandler<TestMessage4>, ILifestyleSingleton
    {
        public TestMessageMultipleHandler()
        {
            _parameters = new List<IMessage>();
        }
        public List<IMessage> _parameters;
        public IReadOnlyCollection<IMessage> Parameters => _parameters;

        public void HandleMessage(TestMessage3 message)
        {
            _parameters.Add(message);
        }

        public ValueTask HandleMessageAsync(TestMessage3 message)
        {
            _parameters.Add(message);
            return new ValueTask();
        }

        public ValueTask HandleMessageAsync(TestMessage4 message)
        {
            _parameters.Add(message);
            return new ValueTask();
        }
    }

    public class TestMessageHandler : IMessageHandler<TestMessage>, ILifestyleSingleton
    {
        public TestMessageHandler()
        {
            _parameters = new List<TestMessage>();
        }
        public List<TestMessage> _parameters;
        public IReadOnlyCollection<TestMessage> Parameters => _parameters;
        public void HandleMessage(TestMessage message)
        {
            _parameters.Add(message);
        }
    }

    public class TestRichMessageHandler : IRichMessageHandler<TestMessage>, ILifestyleSingleton
    {
        public TestRichMessageHandler()
        {
            _parameters = new List<(TestMessage, IMessageDescriptor)>();
        }
        public List<(TestMessage, IMessageDescriptor)> _parameters;
        public IReadOnlyCollection<(TestMessage, IMessageDescriptor)> Parameters => _parameters;
        public void HandleMessage(TestMessage message, IRichMessageDescriptor descriptor)
        {
            _parameters.Add((message, descriptor));
        }
    }


    public class MessageBusTests : TestBase.AbstractionTestBase<MessageBusTests>
    {

        [Fact(DisplayName = "消息总线处理消息")]
        public async Task Should_Trigger()
        {
            await MessageBus.OnMessageReceivedAsync(new TestMessage { Name = "testmsg" }, new RichMessageDescriptor("test.group", "test.topic"));
            Resolve<TestMessageHandler>().Parameters.Single(x => x.Name == "testmsg");
        }

        [Fact(DisplayName = "消息总线处理富消息")]
        public async Task Should_Trigger_Rich()
        {
            await MessageBus.OnMessageReceivedAsync(new TestMessage { Name = "testmsg" }, new RichMessageDescriptor("test.group", "test.topic"));
            Resolve<TestRichMessageHandler>().Parameters.Single(x => x.Item1.Name == "testmsg" && x.Item2.MessageTopic == "test.topic");
        }

        [Fact(DisplayName = "消息总线处理消息单个类实现多消息处理")]
        public async Task Should_Trigger_Multi()
        {
            await MessageBus.OnMessageReceivedAsync(new TestMessage3 { Name = "testmsg3" }, new RichMessageDescriptor("test.group", "test.topic3"));
            await MessageBus.OnMessageReceivedAsync(new TestMessage4 { Name = "testmsg4" }, new RichMessageDescriptor("test.group", "test.topic4"));
            await MessageBus.OnMessageReceivedAsync(new TestMessage4 { Name = "testmsg4-2" }, new RichMessageDescriptor("test.group", "test.topic4-2"));
            await MessageBus.OnMessageReceivedAsync(new TestMessage4 { Name = "testmsg4-3" }, new RichMessageDescriptor("test.group", "test.topic4-3"));
            await MessageBus.OnMessageReceivedAsync(new TestMessage4 { Name = "testmsg4-4" }, new RichMessageDescriptor("test.group", "test.topic4-4"));
            var parameters = Resolve<TestMessageMultipleHandler>().Parameters;
            parameters.Single(x => x is TestMessage3 message && message.Name == "testmsg3");
            parameters.Single(x => x is TestMessage4 message && message.Name == "testmsg4");
        }
    }
}
