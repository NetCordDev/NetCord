namespace NetCord
{
    public class ComponentEmoji : Entity
    {
        private readonly JsonModels.JsonEmoji _jsonEntity;

        public override DiscordId Id => _jsonEntity.Id.GetValueOrDefault(); //
        public string Name => _jsonEntity.Name!;
        public bool Animated => _jsonEntity.Animated;

        internal ComponentEmoji(JsonModels.JsonEmoji jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}
