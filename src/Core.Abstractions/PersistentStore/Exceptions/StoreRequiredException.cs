using System;

namespace Core.PersistentStore
{
    public sealed class StoreRequiredException : PersistentStoreException
    {
        public StoreRequiredException(Type entityType) :this(entityType, "StoreId and StoreName Required but not provided.")
        {
        }

        public StoreRequiredException(Type entityType, string message) : base(entityType, message)
        {
        }

        public StoreRequiredException(Type entityType, string message, Exception innerException) : base(entityType, message, innerException)
        {
        }
    }
}