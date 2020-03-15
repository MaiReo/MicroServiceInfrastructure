using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.PersistentStore.Exceptions
{
    public class EntityKeyRequiredException : PersistentStoreException
    {
        public EntityKeyRequiredException(Type entityType, string message) : base(entityType, message)
        {
        }

        public EntityKeyRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public EntityKeyRequiredException(Type entityType, string message, Exception innerException) : base(entityType, message, innerException)
        {
        }
    }
}
