using System;

namespace Core.KeyValues.AliyunACM
{
    public class AliyunCloudServiceConsts
    {
        public const string SuccessResponseCode = "OK";
        public const string SuccessResponseStatusText = "SUCCESS";
        public static bool IsSuccessCode(string code) => string.Equals(code, SuccessResponseCode, StringComparison.InvariantCultureIgnoreCase);
        public static bool IsSuccessStatus(string statusText) => string.Equals(statusText, SuccessResponseStatusText, StringComparison.InvariantCultureIgnoreCase);

        public class ServiceNames
        {
            public const string Sts = "Sts";

            public const String Acm = "Acm";
        }

        public class Acm
        {
            public const string REGION_ID = "cn-shanghai";
        }
        public class Sts
        {
            public const string REGION_ID = "cn-hangzhou";
            public const string API_VERSION = "2015-04-01";
        }


    }
}
