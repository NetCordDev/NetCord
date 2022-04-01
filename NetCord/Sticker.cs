namespace NetCord
{
    public abstract class Sticker : Entity
    {
        private protected readonly JsonModels.JsonSticker _jsonEntity;

        public override Snowflake Id => _jsonEntity.Id;

        public string Name => _jsonEntity.Name;

        public string Description => _jsonEntity.Description;

        public string Tags => _jsonEntity.Tags;

        public StickerFormat Format => _jsonEntity.Format;

        private protected Sticker(JsonModels.JsonSticker jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}
