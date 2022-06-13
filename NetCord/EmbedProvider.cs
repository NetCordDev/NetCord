namespace NetCord
{
    public class EmbedProvider
    {
        private readonly JsonModels.JsonEmbedProvider _jsonModel;

        public string? Name => _jsonModel.Name;
        public string? Url => _jsonModel.Url;

        public EmbedProvider(JsonModels.JsonEmbedProvider jsonModel)
        {
            _jsonModel = jsonModel;
        }
    }
}