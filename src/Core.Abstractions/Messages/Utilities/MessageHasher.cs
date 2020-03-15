using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Utilities
{
    public class MessageHasher : IMessageHasher
    {
        private readonly IMessageConverter _messageConverter;

        public MessageHasher(IMessageConverter messageConverter)
        {
            _messageConverter = messageConverter;
        }

        public async ValueTask<string> HashAsync(
            IMessageDescriptor descriptor,
            IMessage message,
            HashAlgorithmName algorism = default,
            CancellationToken cancellationToken = default)
        {
            algorism = (algorism == default) ? HashAlgorithmName.SHA1 : algorism;

            byte[] raw = null;

            if (descriptor is IRichMessageDescriptor rich)
            {
                if (rich.Raw != null && rich.Raw.Length != 0)
                {
                    raw = rich.Raw;
                }
                else if (!string.IsNullOrWhiteSpace(rich.MessageId))
                {
                    raw = Encoding.UTF8.GetBytes(rich.MessageId);
                }
            }
            if (raw == null)
            {
                try
                {
                    raw = _messageConverter.Serialize(message);
                }
                catch (Exception)
                {
                }
            }
            if (raw == null)
            {
                return "SRC-ERROR";
            }
            return await Task.Run(async () => await HashBodyAsync(descriptor, raw, algorism, cancellationToken));
        }

        public async ValueTask<string> HashBodyAsync(
            IMessageDescriptor descriptor,
            byte[] messageBody,
            HashAlgorithmName algorism = default,
            CancellationToken cancellationToken = default)
        {
            algorism = (algorism == default) ? HashAlgorithmName.SHA1 : algorism;
            return await Task.Run(() =>
            {
                try
                {
                    using (var hasher = HashAlgorithm.Create(algorism.Name))
                    {
                        var hash = hasher.ComputeHash(messageBody);
                        var hashString = string.Join("", hash.Select(b => b.ToString("x2")));
                        return hashString;
                    }
                }
                catch (Exception e)
                {
                    return $"HASH-ERROR: {e.Message}";
                }
            });
        }
    }
}
