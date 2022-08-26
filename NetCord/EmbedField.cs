namespace NetCord
{
    public class EmbedField
    {
        private readonly JsonModels.JsonEmbedField _jsonModel;

        public string Title => _jsonModel.Title;
        public string Description => _jsonModel.Description;
        public bool? Inline => _jsonModel.Inline;

        public EmbedField(JsonModels.JsonEmbedField jsonModel)
        {
            _jsonModel = jsonModel;
        }
    }
}
