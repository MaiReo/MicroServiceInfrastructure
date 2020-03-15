using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Messages.Utilities
{
    public interface IMessageHasher
    {
        ValueTask<string> HashAsync(IMessageDescriptor descriptor, IMessage message, HashAlgorithmName algorism = default, CancellationToken cancellationToken = default);

        ValueTask<string> HashBodyAsync(IMessageDescriptor descriptor, byte[] messageBody, HashAlgorithmName algorism = default, CancellationToken cancellationToken = default);
    }
}
