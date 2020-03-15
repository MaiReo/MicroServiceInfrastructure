namespace Core.KeyValues
{
    [System.Serializable]
    public class KeyValueException : System.Exception
    {
        public KeyValueException() { }
        public KeyValueException(string message) : this(message, default(string)) { }

        public KeyValueException(string message, string detail) : base(message)
        {
            Detail = detail;
        }
        public KeyValueException(string message, System.Exception inner) : base(message, inner) { }
        protected KeyValueException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public string Detail { get; }
    }
}