using System;
using System.Runtime.Serialization;

namespace Core.PersistentStore
{
    public abstract class PersistentStoreException : Exception
    {
        public PersistentStoreException(Type entityType, string message) : this(entityType, message, null)
        {
        }

        public PersistentStoreException(Type entityType, string message, Exception innerException) : base(message, innerException)
        {
            EntityType = entityType;
        }

        protected PersistentStoreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Type EntityType { get; }
    }
}