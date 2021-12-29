namespace NetCord
{
    public class CategoryChannel : Channel, IGuildChannel
    {
        internal CategoryChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {
            PermissionOverwrites = jsonEntity.PermissionOverwrites.ToImmutableDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
        }

        public string Name => _jsonEntity.Name!;

        public int Position => (int)_jsonEntity.Position!;

        public IReadOnlyDictionary<DiscordId, PermissionOverwrite> PermissionOverwrites { get; }
    }
}
