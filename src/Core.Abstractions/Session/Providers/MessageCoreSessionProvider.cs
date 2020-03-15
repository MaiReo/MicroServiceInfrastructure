using Core.Extensions;
using Core.Messages;
using System.Text;

namespace Core.Session.Providers
{
    public class MessageCoreSessionProvider : ICoreSessionProvider
    {
        private readonly IMessage _message;
        public IRichMessageDescriptor MessageDescriptor { get; }

        public MessageCoreSessionProvider(IMessage message, IRichMessageDescriptor messageDescriptor)
        {
            _message = message;
            MessageDescriptor = messageDescriptor;
        }

        public ICoreSession Session => GetSession();

        private ICoreSession GetSession()
        {
            // if (_message is IMessageWithSession withSessionMessage)
            // {
            //     return ParseSession(withSessionMessage);
            // }
            return ParseSession(MessageDescriptor);
        }

        private ICoreSession ParseSession(IRichMessageDescriptor messageDescriptor)
        {

            if (messageDescriptor.Headers is null)
            {
                return null;
            }
            messageDescriptor.Headers.TryGetValue(SessionConsts.CityId, out var city);
            messageDescriptor.Headers.TryGetValue(SessionConsts.CompanyId, out var companyId);
            messageDescriptor.Headers.TryGetValue(SessionConsts.CompanyName, out var companyName);

            messageDescriptor.Headers.TryGetValue(SessionConsts.DepartmentId, out var departmentId);
            messageDescriptor.Headers.TryGetValue(SessionConsts.DepartmentName, out var departmentName);

            messageDescriptor.Headers.TryGetValue(SessionConsts.BigRegionId, out var bigRegionId);
            messageDescriptor.Headers.TryGetValue(SessionConsts.BigRegionName, out var bigRegionName);

            messageDescriptor.Headers.TryGetValue(SessionConsts.RegionId, out var regionId);
            messageDescriptor.Headers.TryGetValue(SessionConsts.RegionName, out var regionName);

            messageDescriptor.Headers.TryGetValue(SessionConsts.StoreId, out var storeId);
            messageDescriptor.Headers.TryGetValue(SessionConsts.StoreName, out var storeName);

            messageDescriptor.Headers.TryGetValue(SessionConsts.GroupId, out var groupId);
            messageDescriptor.Headers.TryGetValue(SessionConsts.GroupName, out var groupName);

            messageDescriptor.Headers.TryGetValue(SessionConsts.BrokerId, out var brokerId);
            messageDescriptor.Headers.TryGetValue(SessionConsts.BrokerName, out var brokerName);

            messageDescriptor.Headers.TryGetValue(SessionConsts.CurrentUserId, out var currentUserId);
            messageDescriptor.Headers.TryGetValue(SessionConsts.CurrentUserName, out var currentUserName);
            var session = new CoreSession(
                TryConvertFromBytes(city as byte[]),

                TryConvertFromBytes(companyId as byte[]).AsGuidOrNull(), TryUriDecode(TryConvertFromBytes(companyName as byte[])),

                TryConvertFromBytes(departmentId as byte[]).AsGuidOrNull(), TryUriDecode(TryConvertFromBytes(departmentName as byte[])),
                TryConvertFromBytes(bigRegionId as byte[]).AsGuidOrNull(), TryUriDecode(TryConvertFromBytes(bigRegionName as byte[])),
                TryConvertFromBytes(regionId as byte[]).AsGuidOrNull(), TryUriDecode(TryConvertFromBytes(regionName as byte[])),
                TryConvertFromBytes(storeId as byte[]).AsGuidOrNull(), TryUriDecode(TryConvertFromBytes(storeName as byte[])),
                TryConvertFromBytes(groupId as byte[]).AsGuidOrNull(), TryUriDecode(TryConvertFromBytes(groupName as byte[])),

                TryConvertFromBytes(brokerId as byte[]), TryUriDecode(TryConvertFromBytes(brokerName as byte[])),

                TryConvertFromBytes(currentUserId as byte[]), TryUriDecode(TryConvertFromBytes(currentUserName as byte[])));

            return session;
        }


        private string TryConvertFromBytes(byte[] bytes)
        {
            if (bytes is null)
            {
                return default;
            }
            try
            {
                return Encoding.UTF8.GetString(bytes);
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        private string TryUriDecode(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            try
            {
                return System.Web.HttpUtility.UrlDecode(str);
            }
            catch (System.Exception)
            {

            }
            return null;
        }
    }
}
