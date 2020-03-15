using Core.Session;
using System;

namespace Core.TestBase
{
    public class UnitTestCoreSession : ICoreSession
    {
        public UnitTestCoreSession(
            string cityId,
            ICoreSessionContainer<Guid?, string> company,
            ICoreSessionContainer<string, string> user,
            ISessionOrganization organization = default,
            ICoreSessionContainer<string, string> broker = default)
        {
            City = CoreSessionContainer.Create(cityId);
            Company = company;
            User = user ?? CoreSessionContainer.Create(default(string), default(string));
            Organization = organization ?? new SessionOrganization();
            Broker = broker;
        }

        public ICoreSessionContainer<string> City { get; }

        public ICoreSessionContainer<Guid?, string> Company { get; }

        public ICoreSessionContainer<string, string> User { get; }

        public ISessionOrganization Organization { get; }

        [Obsolete]
        public ICoreSessionContainer<Guid?, string> Store => Organization?.Store;

        public ICoreSessionContainer<string, string> Broker { get; }
    }
}
