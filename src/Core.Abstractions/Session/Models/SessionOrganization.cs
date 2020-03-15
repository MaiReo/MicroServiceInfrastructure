using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Session
{
    public class SessionOrganization : ISessionOrganization
    {
        public SessionOrganization()
        {
            Department = CoreSessionContainer.Create(default(Guid?), default);
            BigRegion = CoreSessionContainer.Create(default(Guid?), default);
            Region = CoreSessionContainer.Create(default(Guid?), default);
            Store = CoreSessionContainer.Create(default(Guid?), default);
            Group = CoreSessionContainer.Create(default(Guid?), default);
        }

        public SessionOrganization(
            Guid? departmentId, string departmentName,
            Guid? bigRegionId, string bigRegionName,
            Guid? regionId, string regionName,
            Guid? storeId, string storeName,
            Guid? groupId, string groupName)
        {
            Department = CoreSessionContainer.Create(departmentId, departmentName);
            BigRegion = CoreSessionContainer.Create(bigRegionId, bigRegionName);
            Region = CoreSessionContainer.Create(regionId, regionName);
            Store = CoreSessionContainer.Create(storeId, storeName);
            Group = CoreSessionContainer.Create(groupId, groupName);
        }

        public ICoreSessionContainer<Guid?, string> Department { get; protected set; }

        public ICoreSessionContainer<Guid?, string> BigRegion { get; protected set; }

        public ICoreSessionContainer<Guid?, string> Region { get; protected set; }

        public ICoreSessionContainer<Guid?, string> Store { get; protected set; }

        public ICoreSessionContainer<Guid?, string> Group { get; protected set; }
    }
}
