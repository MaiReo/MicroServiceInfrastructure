using System;

namespace Core.PersistentStore
{
    public sealed class StoreGroupRequiredException : PersistentStoreException
    {
        public StoreGroupRequiredException(Type entityType) :this(entityType, "StoreGroup Required but not provided.")
        {
        }

        public StoreGroupRequiredException(Type entityType, string message) : base(entityType, message)
        {
        }

        public StoreGroupRequiredException(Type entityType, string message, Exception innerException) : base(entityType, message, innerException)
        {
        }
    }
}