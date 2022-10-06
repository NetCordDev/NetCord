using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class DirectoryGuildChannel : Channel, IGuildChannel
{
    public DirectoryGuildChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public string Name => _jsonModel.Name!;

    public int Position => _jsonModel.Position.GetValueOrDefault();

    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    #region Channel
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task ModifyPermissionsAsync(ChannelPermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(Snowflake overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    #endregion
}
