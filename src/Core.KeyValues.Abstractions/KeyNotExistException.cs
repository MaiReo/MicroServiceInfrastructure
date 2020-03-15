namespace Core.KeyValues
{
    [System.Serializable]
    public class KeyNotExistException : KeyValueException
    {
        public KeyNotExistException() { }
        public KeyNotExistException(string message) : this(message, default(string)) { }

        public KeyNotExistException(string message, string detail) : base(message, detail)
        {
        }
        
        public KeyNotExistException(string message, System.Exception inner) : base(message, inner) { }
        protected KeyNotExistException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}