using NetCord.Rest;

namespace NetCord;

public interface IGuildChannel : IEntity
{
    public string Name { get; }
    public int Position { get; }
    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    #region Channel
    public Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null);
    public Task ModifyPermissionsAsync(ChannelPermissionOverwrite permissionOverwrite, RequestProperties? properties = null);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null);
    public Task DeletePermissionAsync(Snowflake overwriteId, RequestProperties? properties = null);
    #endregion
}
