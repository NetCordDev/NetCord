namespace NetCord
{
    public class CategoryChannel : Channel, IGuildChannel
    {
        internal CategoryChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {
            PermissionOverwrites = jsonEntity.PermissionOverwrites.SelectOrEmpty(p => new PermissionOverwrite(p, client));
            if (client.TryGetGuild(jsonEntity.GuildId, out Guild guild))
                Guild = guild;
        }

        public string Name => _jsonEntity.Name;

        public int Position => (int)_jsonEntity.Position;

        public IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }

        public Guild Guild { get; }
    }
}
