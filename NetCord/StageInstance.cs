namespace NetCord
{
    public class StageInstance : ClientEntity
    {
        private readonly JsonModels.JsonStageInstance _jsonEntity;

        public override DiscordId Id => _jsonEntity.Id;
        public DiscordId GuildId => _jsonEntity.GuildId;
        public DiscordId ChannelId => _jsonEntity.ChannelId;
        public string Topic => _jsonEntity.Topic;
        public StageInstancePrivacyLevel PrivacyLevel => _jsonEntity.PrivacyLevel;
        public bool DiscoverableDisabled => _jsonEntity.DiscoverableDisabled;

        internal StageInstance(JsonModels.JsonStageInstance jsonEntity, BotClient client) : base(client)
        {
            _jsonEntity = jsonEntity;
        }
    }
}
