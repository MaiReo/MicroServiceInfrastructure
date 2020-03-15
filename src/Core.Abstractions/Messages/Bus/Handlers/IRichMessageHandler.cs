namespace Core.Messages
{
    public interface IRichMessageHandler<in TMessage> : IMessageHandler where TMessage : IMessage
    {
        void HandleMessage(TMessage message, IRichMessageDescriptor descriptor);
    }
}