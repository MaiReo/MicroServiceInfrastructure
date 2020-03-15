using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Session
{
    public class CoreSession : ICoreSession
    {
        public CoreSession()
        {
        }
        
        public CoreSession(string cityId,
            Guid? companyId, string companyName,
            Guid? departmentId, string departmentName,
            Guid? bigRegionId, string bigRegionName,
            Guid? regionId, string regionName,
            Guid? storeId, string storeName,
            Guid? groupId, string groupName,
            string brokerId, string brokerName,
            string currentUserId, string currentUserName)
        {

            City = CoreSessionContainer.Create(cityId);
            Company = CoreSessionContainer.Create(companyId, companyName);
            Broker = CoreSessionContainer.Create(brokerId, brokerName);
            Organization = new SessionOrganization(
             departmentId, departmentName,
             bigRegionId, bigRegionName,
             regionId, regionName,
             storeId, storeName,
             groupId, groupName);
            User = CoreSessionContainer.Create(currentUserId, currentUserName);
        }

        public ICoreSessionContainer<string> City { get; }

        public ICoreSessionContainer<Guid?, string> Company { get; }

        public ICoreSessionContainer<Guid?, string> Store => Organization?.Store;

        public ICoreSessionContainer<string, string> Broker { get; }

        public ISessionOrganization Organization { get; }

        public ICoreSessionContainer<string, string> User { get; }


    }
}
