namespace NetCord
{
    public class Reference
    {
        private readonly JsonModels.JsonMessageReference _jsonEntity;

        public Snowflake MessageId => _jsonEntity.MessageId.GetValueOrDefault();
        public Snowflake ChannelId => _jsonEntity.ChannelId.GetValueOrDefault();
        public Snowflake? GuildId => _jsonEntity.GuildId;
        public bool? FailIfNotExists => _jsonEntity.FailIfNotExists;

        internal Reference(JsonModels.JsonMessageReference jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}