namespace NetCord
{
    public class Reference
    {
        private readonly JsonModels.JsonMessageReference _jsonEntity;

        public DiscordId MessageId => _jsonEntity.MessageId.GetValueOrDefault();
        public DiscordId ChannelId => _jsonEntity.ChannelId.GetValueOrDefault();
        public DiscordId? GuildId => _jsonEntity.GuildId;
        public bool? FailIfNotExists => _jsonEntity.FailIfNotExists;

        internal Reference(JsonModels.JsonMessageReference jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}