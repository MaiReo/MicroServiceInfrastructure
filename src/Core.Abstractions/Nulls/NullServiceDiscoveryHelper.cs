using Core.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace Core.ServiceDiscovery
{
    public class NullServiceDiscoveryHelper : IServiceDiscoveryHelper
    {
        public void Register()
        {
            //No Actions.
        }

        public ValueTask RegisterAsync()
        {
            return new ValueTask();
        }

        public ValueTask<bool> DeregisterAsync()
        {
            return new ValueTask<bool>(true);
        }

        public ValueTask<string> GetServiceBasePathAsync(string serviceName, string scheme = "http://", CancellationToken cancellationToken = default)
        {
            return new ValueTask<string>(string.Concat(scheme, serviceName));
        }

        public async ValueTask<IDisposableModel<string>> GetServiceBasePathAndAddRefAsync(string serviceName, string scheme = "http://", CancellationToken cancellationToken = default)
        {
            return new DelegateDisposableModel<string>(await GetServiceBasePathAsync(serviceName, scheme, cancellationToken));
        }

        public string GetServiceBasePath(string serviceName, string scheme = "http://")
        {
            return string.Concat(scheme, serviceName);
        }

        public (string Address, int Port) GetServiceAddress(string serviceName)
        {
            return (serviceName, 80);
        }

        public ValueTask<(string Address, int Port)> GetServiceAddressAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            return new ValueTask<(string Address, int Port)>((default, default));
        }

        public async ValueTask<IDisposableModel<(string Address, int Port)>> GetServerAddressAndAddRefAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            return new DelegateDisposableModel<(string Address, int Port)>(await GetServiceAddressAsync(serviceName, cancellationToken));
        }

        public static NullServiceDiscoveryHelper Instance => new NullServiceDiscoveryHelper();

        public bool IsRegistered => false;
    }
}
