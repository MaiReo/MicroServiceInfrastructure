using Core.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace Core.ServiceDiscovery
{
    public interface IServiceDiscoveryHelper
    {
        bool IsRegistered { get; }

        ValueTask RegisterAsync();

        ValueTask<bool> DeregisterAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        /// <exception cref="BadGatewayException"></exception>
        string GetServiceBasePath(string serviceName, string scheme = "http://");

        ValueTask<string> GetServiceBasePathAsync(string serviceName, string scheme = "http://", CancellationToken cancellationToken = default);

        ValueTask<IDisposableModel<string>> GetServiceBasePathAndAddRefAsync(string serviceName, string scheme = "http://", CancellationToken cancellationToken = default);

        (string Address, int Port) GetServiceAddress(string serviceName);

        ValueTask<(string Address, int Port)> GetServiceAddressAsync(string serviceName, CancellationToken cancellationToken = default);

        ValueTask<IDisposableModel<(string Address, int Port)>> GetServerAddressAndAddRefAsync(string serviceName, CancellationToken cancellationToken = default);
    }
}
