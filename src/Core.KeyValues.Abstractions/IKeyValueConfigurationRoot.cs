using System.Collections.Generic;

namespace Core.KeyValues
{
    public interface IKeyValueConfigurationRoot
    {
        IReadOnlyCollection<IKeyValueConfiguration> Configuration { get; }
    }
}