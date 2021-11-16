namespace NetCord
{
    public class MessageEmbedProvider
    {
        private readonly JsonModels.JsonEmbedProvider _jsonEntity;

        public string? Name => _jsonEntity.Name;
        public string? Url => _jsonEntity.Url;

        internal MessageEmbedProvider(JsonModels.JsonEmbedProvider jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}