using Core.Extensions;
using System.Collections.Generic;

namespace Core.Session.Extensions
{
    public static class CoreSessionExtensions
    {
        public static string GetCurrentUserId(this ICoreSession session) => session?.User?.Id;

        public static string GetCurrentUserName(this ICoreSession session) => session?.User?.Name;


        public static IDictionary<string, string> ToHeaders(this ICoreSession session)
        {
            if (session is null)
            {
                return null;
            }
            var o = session.Organization;

            var dictionary = new Dictionary<string, string>
            {
                { SessionConsts.CityId, session?.City?.Id },

                { SessionConsts.CompanyId, session?.Company?.Id?.AsStringOrDefault() },
                { SessionConsts.CompanyName, session?.Company?.Name },

                { SessionConsts.DepartmentId, o?.Department?.Id?.AsStringOrDefault() },
                { SessionConsts.DepartmentName, o?.Department?.Name },

                { SessionConsts.BigRegionId, o?.BigRegion?.Id?.AsStringOrDefault() },
                { SessionConsts.BigRegionName, o?.BigRegion?.Name },

                { SessionConsts.RegionId, o?.Region?.Id?.AsStringOrDefault() },
                { SessionConsts.RegionName, o?.Region?.Name },

                { SessionConsts.StoreId, o?.Store?.Id?.AsStringOrDefault() },
                { SessionConsts.StoreName, o?.Store?.Name },

                { SessionConsts.GroupId, o?.Group?.Id?.AsStringOrDefault() },
                { SessionConsts.GroupName, o?.Group?.Name },

                { SessionConsts.BrokerId, session?.Broker?.Id },
                { SessionConsts.BrokerName, session?.Broker?.Name },

                { SessionConsts.CurrentUserId, session?.User?.Id },
                { SessionConsts.CurrentUserName, session?.User?.Name }
            };

            return dictionary;
        }

        public static ICoreSession ToSession(this IDictionary<string, string> dictionary)
        {
            if (dictionary is null)
            {
                return null;
            }
            dictionary.TryGetValue(SessionConsts.CityId, out var cityId);
            dictionary.TryGetValue(SessionConsts.CompanyId, out var companyId);
            dictionary.TryGetValue(SessionConsts.CompanyName, out var companyName);

            dictionary.TryGetValue(SessionConsts.DepartmentId, out var departmentId);
            dictionary.TryGetValue(SessionConsts.DepartmentName, out var DepartmentName);

            dictionary.TryGetValue(SessionConsts.BigRegionId, out var bigRegionId);
            dictionary.TryGetValue(SessionConsts.BigRegionName, out var bigRegionName);

            dictionary.TryGetValue(SessionConsts.RegionId, out var regionId);
            dictionary.TryGetValue(SessionConsts.RegionName, out var regionName);

            dictionary.TryGetValue(SessionConsts.StoreId, out var storeId);
            dictionary.TryGetValue(SessionConsts.StoreName, out var storeName);

            dictionary.TryGetValue(SessionConsts.GroupId, out var groupId);
            dictionary.TryGetValue(SessionConsts.GroupName, out var groupName);

            dictionary.TryGetValue(SessionConsts.BrokerId, out var brokerId);
            dictionary.TryGetValue(SessionConsts.BrokerName, out var brokerName);

            
            dictionary.TryGetValue(SessionConsts.CurrentUserId, out var currentUserId);
            dictionary.TryGetValue(SessionConsts.CurrentUserName, out var currentUserName);

            var session = new CoreSession(
                cityId,
                companyId?.AsGuidOrNull(), companyName,
                departmentId.AsGuidOrNull(), DepartmentName,
                bigRegionId.AsGuidOrNull(),bigRegionName,
                regionId.AsGuidOrNull(),regionName,
                storeId?.AsGuidOrNull(), storeName,
                groupId.AsGuidOrNull(),groupName,
                brokerId, brokerName,
                currentUserId, currentUserName);

            return session;
        }
    }
}
