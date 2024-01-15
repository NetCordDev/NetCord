using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

internal class UnknownGuildChannel : UnknownChannel, IUnknownGuildChannel
{
    public UnknownGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public ulong GuildId { get; }

    public int? Position => _jsonModel.Position;

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }

    public string Name => _jsonModel.Name!;

    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(ulong overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task ModifyPermissionsAsync(PermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
}
