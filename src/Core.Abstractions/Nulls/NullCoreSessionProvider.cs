namespace Core.Session
{
    public class NullCoreSessionProvider : ICoreSessionProvider
    {
        public ICoreSession Session => NullCoreSession.Instance;
        public static NullCoreSessionProvider Instance => new NullCoreSessionProvider();

    }
}
