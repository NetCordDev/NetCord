namespace NetCord
{
    public class Reference
    {
        private readonly JsonModels.JsonMessageReference _jsonModel;

        public Snowflake MessageId => _jsonModel.MessageId.GetValueOrDefault();
        public Snowflake ChannelId => _jsonModel.ChannelId.GetValueOrDefault();
        public Snowflake? GuildId => _jsonModel.GuildId;
        public bool? FailIfNotExists => _jsonModel.FailIfNotExists;

        public Reference(JsonModels.JsonMessageReference jsonModel)
        {
            _jsonModel = jsonModel;
        }
    }
}