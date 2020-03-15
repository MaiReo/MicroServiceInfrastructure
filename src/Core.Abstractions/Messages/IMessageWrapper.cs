namespace Core.Messages
{
    public interface IMessageWrapper
    {
        IMessage Message { get; }
        IMessageDescriptor Descriptor { get; }
    }
}