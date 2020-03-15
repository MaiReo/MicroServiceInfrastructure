using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Messages.Fake
{
    public class FakeMessagePublisherWrapper : IMessagePublisherWrapper
    {
        public FakeMessagePublisherWrapper()
        {
            PublishParameters = new List<IMessageWrapper>();
        }
        public List<IMessageWrapper> PublishParameters { get; }

        public void Publish(IMessageWrapper messageWrapper)
        {
            PublishParameters.Add(messageWrapper);
        }

        public async ValueTask PublishAsync(IMessageWrapper messageWrapper)
        {
            await Task.Run(() => Publish(messageWrapper));
        }
    }
}
