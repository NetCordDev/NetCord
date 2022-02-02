namespace NetCord
{
    public class GroupDMChannel : DMChannel
    {
        public string Name => _jsonEntity.Name!;
        public string? IconHash => _jsonEntity.IconHash;
        public DiscordId OwnerId => _jsonEntity.OwnerId.GetValueOrDefault();
        public DiscordId? ApplicationId => _jsonEntity.ApplicationId;

        internal GroupDMChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
        {
        }

        public async Task<GroupDMChannel> ModifyAsync(Action<GroupDMChannelOptions> action, RequestOptions? options = null) => (GroupDMChannel)await _client.Channel.ModifyAsync(Id, action, options).ConfigureAwait(false);
    }
}