using System.Collections.Generic;
using System.Linq;

namespace Core.KeyValues
{
    public class KeyValueConfigurationRoot : IKeyValueConfigurationRoot
    {
        public KeyValueConfigurationRoot(IEnumerable<IKeyValueConfiguration> configurations = default)
        {
            Configuration = (configurations ?? Enumerable.Empty<IKeyValueConfiguration>()).ToList();
        }
        public IReadOnlyCollection<IKeyValueConfiguration> Configuration { get; }
    }
}