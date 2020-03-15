using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utilities
{
    public class RandomServiceEndpointSelector : IServiceEndpointSelector
    {
        public RandomServiceEndpointSelector()
        {
            _random = new Random();
        }
        private readonly Random _random;

        public IDisposableModel<(string Address, int Port)> SelectService(IEnumerable<(string Address, int Port)> services)
        {
            var service = services.ElementAtOrDefault(_random.Next(0, services.Count()));
            return new DelegateDisposableModel<(string Address, int Port)>(service);
        }
    }
}
