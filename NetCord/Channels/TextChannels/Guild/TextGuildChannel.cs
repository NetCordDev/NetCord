namespace NetCord
{
    public class TextGuildChannel : TextChannel, IGuildChannel
    {
        public DiscordId GuildId => _jsonEntity.GuildId;
        public DiscordId? CategoryId => _jsonEntity.ParentId;
        public string? Topic => _jsonEntity.Topic;
        public bool IsNsfw => _jsonEntity.IsNsfw;
        public int Slowmode => (int)_jsonEntity.Slowmode;

        public string Name => _jsonEntity.Name;

        public int Position => (int)_jsonEntity.Position;

        public IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }

        public Guild Guild { get; }

        internal TextGuildChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {
            PermissionOverwrites = jsonEntity.PermissionOverwrites.SelectOrEmpty(p => new PermissionOverwrite(p, client));
            if (client.TryGetGuild(jsonEntity.GuildId, out Guild guild))
                Guild = guild;
        }
    }
}
