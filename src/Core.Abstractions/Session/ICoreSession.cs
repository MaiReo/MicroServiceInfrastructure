namespace Core.Session
{

    public interface ICoreSession
    {
        ICoreSessionContainer<string> City { get; }

        ICoreSessionContainer<System.Guid?, string> Company { get; }

        [System.Obsolete("Use Organization.Store instead of this one")]
        ICoreSessionContainer<System.Guid?, string> Store { get; }

        ICoreSessionContainer<string, string> Broker { get; }

        ISessionOrganization Organization { get; }

        ICoreSessionContainer<string, string> User { get; }
    }
}
