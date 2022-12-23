using NetCord.Rest;

namespace NetCord;

public interface IGuildChannel : IEntity
{
    public ulong? GuildId { get; }
    public string Name { get; }
    public int Position { get; }
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }

    #region Channel
    public Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null);
    public Task ModifyPermissionsAsync(PermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null);
    public Task DeletePermissionAsync(ulong overwriteId, RequestProperties? properties = null);
    #endregion
}
