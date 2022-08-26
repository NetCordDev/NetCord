namespace NetCord
{
    public class EmbedAuthor
    {
        private readonly JsonModels.JsonEmbedAuthor _jsonModel;

        public string? Name => _jsonModel.Name;
        public string? Url => _jsonModel.Url;
        public string? IconUrl => _jsonModel.IconUrl;
        public string? ProxyIconUrl => _jsonModel.ProxyIconUrl;

        public EmbedAuthor(JsonModels.JsonEmbedAuthor jsonModel)
        {
            _jsonModel = jsonModel;
        }
    }
}
