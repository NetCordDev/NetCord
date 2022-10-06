using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class ForumGuildChannel : Channel, IGuildChannel
{
    public ForumGuildChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
        AvailableTags = jsonModel.AvailableTags!.Select(t => new ForumTag(t));
        if (jsonModel.DefaultReactionEmoji != null)
            DefaultReactionEmoji = new(jsonModel.DefaultReactionEmoji);
    }

    public string Name => _jsonModel.Name!;
    public int Position => _jsonModel.Position.GetValueOrDefault();
    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }
    public IEnumerable<ForumTag> AvailableTags { get; }
    public DefaultReaction? DefaultReactionEmoji { get; }
    public int? DefaultThreadRateLimitPerUser => _jsonModel.DefaultThreadRateLimitPerUser;
    public SortOrderType DefaultSortOrder => _jsonModel.DefaultSortOrder.GetValueOrDefault();

    #region Channel
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task<ForumGuildThread> CreateThreadAsync(Snowflake channelId, ForumGuildThreadProperties threadProperties, RequestProperties? properties = null) => _client.CreateForumGuildThreadAsync(channelId, threadProperties, properties);
    public Task ModifyPermissionsAsync(ChannelPermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(Snowflake overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    #endregion
}
