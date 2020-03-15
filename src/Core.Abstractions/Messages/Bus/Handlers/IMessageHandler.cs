namespace Core.Messages
{
    public interface IMessageHandler
    {
    }

    public interface IMessageHandler<in TMessage> : IMessageHandler where TMessage : IMessage
    {
        void HandleMessage(TMessage message);
    }
}