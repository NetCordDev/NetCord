namespace NetCord
{
    public class GroupDMChannel : DMChannel
    {
        public string Name => _jsonEntity.Name!;
        public string? IconHash => _jsonEntity.IconHash;
        public DiscordId OwnerId => _jsonEntity.OwnerId!;
        public DiscordId? ApplicationId => _jsonEntity.ApplicationId;

        internal GroupDMChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
        {
        }
    }
}