namespace NetCord
{
    public class EmbedProvider
    {
        private readonly JsonModels.JsonEmbedProvider _jsonEntity;

        public string? Name => _jsonEntity.Name;
        public string? Url => _jsonEntity.Url;

        internal EmbedProvider(JsonModels.JsonEmbedProvider jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}