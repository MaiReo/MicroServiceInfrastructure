using System;

namespace Core.PersistentStore
{
    public interface IMustHaveBroker
    {
        string BrokerId { get; set; }

        string BrokerName { get; set; }
    }
}
