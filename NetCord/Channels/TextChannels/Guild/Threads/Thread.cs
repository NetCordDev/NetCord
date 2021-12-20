namespace NetCord
{
    //[JsonConverter(typeof(SocketThreadConverter))]
    public abstract class Thread : TextGuildChannel
    {
        public ThreadMetadata Metadata { get; }
        public ThreadSelfUser? CurrentUser { get; }
        public int DefaultAutoArchiveDuration => (int)_jsonEntity.DefaultAutoArchiveDuration!;
        public DiscordId OwnerId => _jsonEntity.OwnerId!;

        public Task<IReadOnlyDictionary<DiscordId, ThreadUser>> GetUsersAsync() => _client.Rest.Channel.GetThreadUsersAsync(Id);

        internal Thread(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {
            Metadata = new(jsonEntity.Metadata);
            CurrentUser = new(jsonEntity.CurrentUser);
        }
    }
}
