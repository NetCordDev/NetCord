namespace NetCord
{
    public class GuildSticker : Sticker
    {
        private readonly BotClient _client;

        public bool? Available => _jsonEntity.Available;

        public DiscordId GuildId => _jsonEntity.GuildId;

        public User Creator { get; }

        internal GuildSticker(JsonModels.JsonSticker jsonEntity, BotClient client) : base(jsonEntity)
        {
            _client = client;
            Creator = new(jsonEntity.Creator, client);
        }
    }
}
