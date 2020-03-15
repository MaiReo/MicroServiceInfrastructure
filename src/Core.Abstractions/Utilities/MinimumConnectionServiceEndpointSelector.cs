using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Utilities
{
    public class MinimumConnectionServiceEndpointSelector : IServiceEndpointSelector
    {
        private readonly ConcurrentDictionary<(string Address, int Port), long> _connections;

        public MinimumConnectionServiceEndpointSelector()
        {
            _connections = new ConcurrentDictionary<(string Address, int Port), long>();
        }

        public IDisposableModel<(string Address, int Port)> SelectService(IEnumerable<(string Address, int Port)> services)
        {
            var service = services.OrderByDescending(svc => _connections.GetOrAdd(svc, 0)).FirstOrDefault();

            var currentValue = _connections.GetOrAdd(service, 0);
            _connections.TryUpdate(service, currentValue + 1, currentValue);

            return new DelegateDisposableModel<(string, int)>(service, () =>
            {
                var current = _connections.GetOrAdd(service, 0);
                if (current != 0)
                {
                    _connections.TryUpdate(service, current - 1, current);
                }
            });
        }
    }
}
