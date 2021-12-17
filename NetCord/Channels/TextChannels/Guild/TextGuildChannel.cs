namespace NetCord
{
    public class TextGuildChannel : TextChannel, IGuildChannel
    {
        public DiscordId? CategoryId => _jsonEntity.ParentId;
        public string? Topic => _jsonEntity.Topic;
        public bool IsNsfw => _jsonEntity.IsNsfw;
        public int Slowmode => (int)_jsonEntity.Slowmode!;

        public string Name => _jsonEntity.Name!;

        public int Position => (int)_jsonEntity.Position!;

        public IReadOnlyDictionary<DiscordId, PermissionOverwrite> PermissionOverwrites { get; }

        internal TextGuildChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {
            PermissionOverwrites = jsonEntity.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
        }
    }
}
