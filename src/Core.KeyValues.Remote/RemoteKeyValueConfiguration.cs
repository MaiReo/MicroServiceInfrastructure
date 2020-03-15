using System;
using System.Collections.Generic;
using System.Text;

namespace Core.KeyValues.Remote
{
    public class RemoteKeyValueConfiguration : IKeyValueConfiguration
    {
        public Uri BaseUri { get; set; }
    }
}
