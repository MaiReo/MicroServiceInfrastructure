using System;

namespace Core.PersistentStore
{
    public sealed class CompanyRequiredException : PersistentStoreException
    {
        public CompanyRequiredException(Type entityType) :this(entityType, "CompanyId and CompanyName Required but not provided.")
        {
        }

        public CompanyRequiredException(Type entityType, string message) : base(entityType, message)
        {
        }

        public CompanyRequiredException(Type entityType, string message, Exception innerException) : base(entityType, message, innerException)
        {
        }
    }
}