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

        //public MessageReference(DiscordId messageId)
        //{
        //    MessageId = messageId;
        //}

        //public MessageReference(Message message)
        //{
        //    MessageId = message.Id;
        //}

        //public static implicit operator MessageReference(Message message)
        //{
        //    return new(message);
        //}

        //public static implicit operator MessageReference(DiscordId messageId)
        //{
        //    return new(messageId);
        //}
    }
}