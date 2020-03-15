using Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net;

namespace Core.Session
{
    public class HttpHeaderCoreSession : ICoreSession
    {
        private readonly Func<IHeaderDictionary> _headerAccessor;

        public HttpHeaderCoreSession(Func<IHeaderDictionary> headerAccessor)
        {
            _headerAccessor = headerAccessor;
        }

        public ICoreSessionContainer<string> City => CoreSessionContainer.Create(GetHeaderValue(_headerAccessor?.Invoke(), SessionConsts.CityId));

        public ICoreSessionContainer<Guid?, string> Company
        {
            get
            {
                var headers = _headerAccessor?.Invoke();
                return CoreSessionContainer.Create(
                    GetHeaderValue(headers, SessionConsts.CompanyId).AsGuidOrNull(),
                    GetHeaderValue(headers, SessionConsts.CompanyName, true)
                );
            }
        }

        [Obsolete]
        public ICoreSessionContainer<Guid?, string> Store => Organization?.Store;

        public ICoreSessionContainer<string, string> Broker
        {
            get
            {
                var headers = _headerAccessor?.Invoke();
                return CoreSessionContainer.Create(
                    GetHeaderValue(headers, SessionConsts.BrokerId),
                    GetHeaderValue(headers, SessionConsts.BrokerName, true)
                );
            }
        }
        public ISessionOrganization Organization
        {
            get
            {
                var headers = _headerAccessor?.Invoke();
                return new SessionOrganization(
                    GetHeaderValue(headers, SessionConsts.DepartmentId).AsGuidOrNull(),
                    GetHeaderValue(headers, SessionConsts.DepartmentName, true),
                    GetHeaderValue(headers, SessionConsts.BigRegionId).AsGuidOrNull(),
                    GetHeaderValue(headers, SessionConsts.BigRegionName, true),
                    GetHeaderValue(headers, SessionConsts.RegionId).AsGuidOrNull(),
                    GetHeaderValue(headers, SessionConsts.RegionName, true),
                    GetHeaderValue(headers, SessionConsts.StoreId).AsGuidOrNull(),
                    GetHeaderValue(headers, SessionConsts.StoreName, true),
                    GetHeaderValue(headers, SessionConsts.GroupId).AsGuidOrNull(),
                    GetHeaderValue(headers, SessionConsts.GroupName, true)
                );
            }
        }

        public ICoreSessionContainer<string, string> User
        {
            get
            {
                var headers = _headerAccessor?.Invoke();
                return CoreSessionContainer.Create(
                    GetHeaderValue(headers, SessionConsts.CurrentUserId),
                    GetHeaderValue(headers, SessionConsts.CurrentUserName, true)
                );
            }
        }

        private string GetHeaderValue(IHeaderDictionary headers, string key, bool urlDecode = false)
        {
            if (headers == null)
            {
                return default;
            }
            if (headers.TryGetValue(key, out var value))
            {
                if (urlDecode)
                {
                    return TryUrlDecode(value);
                }
                return value.FirstOrDefault();
            }
            return default;
        }


        private string TryUrlDecode(StringValues values)
        {
            var urlEncoded = values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(urlEncoded))
            {
                return default;
            }
            try
            {
                var urlDecoded = WebUtility.UrlDecode(urlEncoded);
                return urlDecoded;
            }
            catch
            {

            }
            return default;
        }

    }
}
