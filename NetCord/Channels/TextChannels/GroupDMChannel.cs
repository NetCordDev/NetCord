using NetCord.Rest;

namespace NetCord;

public class GroupDMChannel : DMChannel
{
    public string Name => _jsonModel.Name!;
    public string? IconHash => _jsonModel.IconHash;
    public Snowflake OwnerId => _jsonModel.OwnerId.GetValueOrDefault();
    public Snowflake? ApplicationId => _jsonModel.ApplicationId;

    public GroupDMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    #region Channel
    public async Task<GroupDMChannel> ModifyAsync(Action<GroupDMChannelOptions> action, RequestProperties? properties = null) => (GroupDMChannel)await _client.ModifyGroupDMChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task AddUserAsync(Snowflake userId, GroupDMUserAddProperties groupDMUserAddProperties, RequestProperties? properties = null) => _client.GroupDMChannelAddUserAsync(Id, userId, groupDMUserAddProperties, properties);
    public Task DeleteUserAsync(Snowflake userId, RequestProperties? properties = null) => _client.GroupDMChannelDeleteUserAsync(Id, userId, properties);
    #endregion
}
