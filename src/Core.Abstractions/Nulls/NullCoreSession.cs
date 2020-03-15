using System;

namespace Core.Session
{
    public class NullCoreSession : ICoreSession
    {
        public NullCoreSession()
        {

        }

        public ICoreSessionContainer<string> City => CoreSessionContainer.Create(default(string));

        public ICoreSessionContainer<Guid?, string> Company => CoreSessionContainer.Create(default(Guid?), default);

        [Obsolete]
        public ICoreSessionContainer<Guid?, string> Store => Organization?.Store;

        public ICoreSessionContainer<string, string> Broker => CoreSessionContainer.Create(default(string), default);

        public ISessionOrganization Organization => new SessionOrganization();

        public ICoreSessionContainer<string, string> User => CoreSessionContainer.Create(default(string), default);

        public static NullCoreSession Instance => new NullCoreSession();
    }
}