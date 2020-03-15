using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities
{
    public interface IServiceEndpointSelector
    {
        IDisposableModel<(string Address, int Port)> SelectService(IEnumerable<(string Address, int Port)> services);
    }
}
