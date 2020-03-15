using System;
namespace Core.KeyValues.AliyunACM.Internal
{
    internal class AliyunStsResponseModel
    {
        public const string HTTP_ENDPOINT = "http://100.100.100.200/latest/meta-data/ram/security-credentials/";
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public string SecurityToken { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public string Code { get; set; }
    }
}