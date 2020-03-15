using System.Threading.Tasks;

namespace Core.Messages
{
    public interface IAsyncRichMessageHandler<in TMessage> : IMessageHandler where TMessage : IMessage
    {
        ValueTask HandleMessageAsync(TMessage message, IRichMessageDescriptor descriptor);
    }
}