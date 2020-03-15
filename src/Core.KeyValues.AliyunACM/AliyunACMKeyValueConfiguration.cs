using System;
using Core.KeyValues;

namespace Core.KeyValues.AliyunACM
{
    public class AliyunACMKeyValueConfiguration : IKeyValueConfiguration
    {
        public AliyunACMKeyValueConfiguration(
            string endpoint,
            string @namespace,
            string ramRoleName = default,
            string accessKey = default,
            string secretKey = default)
        {
            Endpoint = endpoint;
            Namespace = @namespace;
            RamRoleName = ramRoleName;
            AccessKey = accessKey;
            SecretKey = secretKey;
        }

        public string Endpoint { get; }

        public string Namespace { get; }

        public string RamRoleName { get; }

        public string AccessKey { get; }

        public string SecretKey { get; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Endpoint))
            {
                throw new InvalidOperationException("缺少阿里云ACM必要配置:终结点");
            }
            if (string.IsNullOrWhiteSpace(this.Namespace))
            {
                throw new InvalidOperationException("缺少阿里云ACM必要配置:命名空间");
            }
            if (string.IsNullOrWhiteSpace(this.RamRoleName))
            {
                if (string.IsNullOrWhiteSpace(this.AccessKey))
                {
                    throw new InvalidOperationException("未指定ECS的RAM角色时必须指定访问key");
                }
                else if (string.IsNullOrWhiteSpace(this.SecretKey))
                {
                    throw new InvalidOperationException("未指定ECS的RAM角色时必须指定安全key");
                }
                return;
            }
        }

        public bool IsStsEnabled()
        {
            if (string.IsNullOrWhiteSpace(this.RamRoleName))
            {
                return false;
            }
            var noAk = string.IsNullOrWhiteSpace(this.AccessKey);
            var noSk = string.IsNullOrWhiteSpace(this.SecretKey);
            return noAk && noSk;
        }
    }
}