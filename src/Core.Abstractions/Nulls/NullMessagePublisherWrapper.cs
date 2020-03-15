using System.Threading.Tasks;

namespace Core.Messages
{
    public class NullMessagePublisherWrapper : IMessagePublisherWrapper
    {
        public void Publish(IMessageWrapper messageWrapper)
        {
            // No Actions.
        }

        public ValueTask PublishAsync(IMessageWrapper messageWrapper)
        {
            // No Actions.
            return new ValueTask();
        }

        public static NullMessagePublisherWrapper Instance => new NullMessagePublisherWrapper();
    }
}