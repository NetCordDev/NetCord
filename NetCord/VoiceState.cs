namespace NetCord
{
    public class VoiceState
    {
        internal JsonModels.JsonVoiceState _jsonEntity;

        public Snowflake? GuildId => _jsonEntity.GuildId;

        public Snowflake? ChannelId => _jsonEntity.ChannelId;

        public Snowflake UserId => _jsonEntity.UserId;

        //public GuildUser? User => _jsonEntity.User;

        public string SessionId => _jsonEntity.SessionId;

        public bool IsDeafened => _jsonEntity.IsDeafened;

        public bool IsMuted => _jsonEntity.IsMuted;

        public bool IsSelfDeafened => _jsonEntity.IsSelfDeafened;

        public bool IsSelfMuted => _jsonEntity.IsSelfMuted;

        public bool? SelfStreamExists => _jsonEntity.SelfStreamExists;

        public bool SelfVideoExists => _jsonEntity.SelfVideoExists;

        public bool Suppressed => _jsonEntity.Suppressed;

        public DateTimeOffset? RequestToSpeakTimestamp => _jsonEntity.RequestToSpeakTimestamp;

        internal VoiceState(JsonModels.JsonVoiceState jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}
