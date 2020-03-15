using System.Collections.Generic;

namespace Core.PersistentStore
{
    [System.Serializable]
    public class ElasticSearchOperationIsNotValidException : System.Exception
    {
        public ElasticSearchOperationIsNotValidException() { }

        public ElasticSearchOperationIsNotValidException(string message, IEnumerable<ElasticSearchOperationIsNotValidException> innerExceptions) : base(message, new System.AggregateException(innerExceptions))
        {
        }
        public ElasticSearchOperationIsNotValidException(string message, string index) : this(message, index, null)
        {
        }
        public ElasticSearchOperationIsNotValidException(string message, string index, System.Exception inner) : base(message, inner)
        {
            this.Index = index;
        }
        protected ElasticSearchOperationIsNotValidException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public string Index { get; }

    }
}