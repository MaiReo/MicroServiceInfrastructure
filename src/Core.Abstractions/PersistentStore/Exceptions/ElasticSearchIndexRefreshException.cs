namespace Core.PersistentStore.Repositories
{
    [System.Serializable]
    public class ElasticSearchIndexRefreshException : ElasticSearchOperationIsNotValidException
    {
        public ElasticSearchIndexRefreshException(string message, string index, System.Exception inner) : base(message, index, inner)
        {
        }
        protected ElasticSearchIndexRefreshException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }
}