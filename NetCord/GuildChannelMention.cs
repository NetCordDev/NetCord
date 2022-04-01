namespace NetCord
{
    public class GuildChannelMention : Entity
    {
        private readonly JsonModels.JsonGuildChannelMention _jsonEntity;

        public override Snowflake Id => _jsonEntity.Id;
        public Snowflake GuildId => _jsonEntity.GuildId;
        public ChannelType Type => _jsonEntity.Type;
        public string Name => _jsonEntity.Name;

        internal GuildChannelMention(JsonModels.JsonGuildChannelMention jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}