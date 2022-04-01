namespace NetCord;

public class GroupDMChannel : DMChannel
{
    public string Name => _jsonEntity.Name!;
    public string? IconHash => _jsonEntity.IconHash;
    public Snowflake OwnerId => _jsonEntity.OwnerId.GetValueOrDefault();
    public Snowflake? ApplicationId => _jsonEntity.ApplicationId;

    internal GroupDMChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
    }

    public async Task<GroupDMChannel> ModifyAsync(Action<GroupDMChannelOptions> action, RequestProperties? options = null) => (GroupDMChannel)await _client.ModifyChannelAsync(Id, action, options).ConfigureAwait(false);
}