namespace NetCord
{
    public abstract class StickerBase : Entity
    {
        private protected readonly JsonModels.JsonSticker _jsonEntity;

        public override DiscordId Id => _jsonEntity.Id;

        public string Name => _jsonEntity.Name;

        public string Description => _jsonEntity.Description;

        public string Tags => _jsonEntity.Tags;

        public StickerFormat Format => _jsonEntity.Format;

        private protected StickerBase(JsonModels.JsonSticker jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}
