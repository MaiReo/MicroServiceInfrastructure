namespace Core.PersistentStore.Repositories
{
    [System.Serializable]
    public class ElasticSearchDocumentCreateException<TId> : ElasticSearchOperationIsNotValidException
    {
        public ElasticSearchDocumentCreateException(string message, string index, TId id, object document, System.Exception inner) : base(message, index, inner)
        {
            this.Id = id;
            this.Document = document;
        }
        protected ElasticSearchDocumentCreateException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }


        public TId Id { get; }

        public object Document { get; }

    }
}