using System;

namespace Core.PersistentStore
{
    public sealed class CityRequiredException : PersistentStoreException
    {
        public CityRequiredException(Type entityType) :this(entityType, "CityId Required but not provided.")
        {
        }

        public CityRequiredException(Type entityType, string message) : base(entityType, message)
        {
        }

        public CityRequiredException(Type entityType, string message, Exception innerException) : base(entityType, message, innerException)
        {
        }
    }
}