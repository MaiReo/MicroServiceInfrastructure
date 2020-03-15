namespace Core.Messages.Bus
{
    public interface IMessageWithInheritableGenericArgument
    {
        object[] GetConstructorArgs();
    }
}