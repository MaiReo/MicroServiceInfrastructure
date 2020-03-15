using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.PersistentStore.Exceptions
{
    public class EntityKeyDuplicateException : PersistentStoreException
    {
        public EntityKeyDuplicateException(Type entityType, string message) : base(entityType, message)
        {
        }

        public EntityKeyDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public EntityKeyDuplicateException(Type entityType, string message, Exception innerException) : base(entityType, message, innerException)
        {
        }
    }
}
