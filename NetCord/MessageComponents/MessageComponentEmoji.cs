namespace NetCord
{
    public class MessageComponentEmoji : Entity
    {
        private readonly JsonModels.JsonEmoji _jsonEntity;

        public override DiscordId Id => _jsonEntity.Id;
        public string Name => _jsonEntity.Name;
        public bool Animated => _jsonEntity.Animated;

        internal MessageComponentEmoji(JsonModels.JsonEmoji jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}
