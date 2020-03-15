using System;

namespace Core.PersistentStore
{
    public sealed class BrokerRequiredException : PersistentStoreException
    {
        public BrokerRequiredException(Type entityType) :this(entityType, "BrokerId and BrokerName Required but not provided.")
        {
        }

        public BrokerRequiredException(Type entityType, string message) : base(entityType, message)
        {
        }

        public BrokerRequiredException(Type entityType, string message, Exception innerException) : base(entityType, message, innerException)
        {
        }
    }
}