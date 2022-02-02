namespace NetCord
{
    public class EmbedField
    {
        private readonly JsonModels.JsonEmbedField _jsonEntity;

        public string Title => _jsonEntity.Title;
        public string Description => _jsonEntity.Description;
        public bool? Inline => _jsonEntity.Inline;

        internal EmbedField(JsonModels.JsonEmbedField jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}