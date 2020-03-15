using System;
using System.Runtime.Serialization;

namespace Core.PersistentStore.Exceptions
{
    public class EntityNotFoundException : PersistentStoreException
    {
        public EntityNotFoundException(Type entityType, string message) : base(entityType, message)
        {
        }

        public EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public EntityNotFoundException(Type entityType, string message, Exception innerException) : base(entityType, message, innerException)
        {
        }
    }
}
