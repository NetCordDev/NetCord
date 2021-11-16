namespace NetCord
{
    public class MessageEmbedAuthor
    {
        private readonly JsonModels.JsonEmbedAuthor _jsonEntity;

        public string? Name => _jsonEntity.Name;
        public string? Url => _jsonEntity.Url;
        public string? IconUrl => _jsonEntity.IconUrl;
        public string? ProxyIconUrl => _jsonEntity.ProxyIconUrl;

        internal MessageEmbedAuthor(JsonModels.JsonEmbedAuthor jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}