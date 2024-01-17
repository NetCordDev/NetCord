using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class ForumGuildChannel : Channel, IGuildChannel
{
    public ForumGuildChannel(JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
        AvailableTags = jsonModel.AvailableTags.SelectOrEmpty(t => new ForumTag(t)).ToArray();

        var defaultReactionEmoji = jsonModel.DefaultReactionEmoji;
        if (defaultReactionEmoji is not null)
            DefaultReactionEmoji = new(defaultReactionEmoji);
    }

    public ulong GuildId { get; }
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
    public string Name => _jsonModel.Name!;
    public string? Topic => _jsonModel.Topic;
    public bool Nsfw => _jsonModel.Nsfw.GetValueOrDefault();
    public ulong? LastMessageId => _jsonModel.LastMessageId;
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();
    public ulong? ParentId => _jsonModel.ParentId;
    public DateTimeOffset? LastPin => _jsonModel.LastPin;
    public IReadOnlyList<ForumTag> AvailableTags { get; }
    public ForumGuildChannelDefaultReaction? DefaultReactionEmoji { get; }
    public int DefaultThreadSlowmode => _jsonModel.DefaultThreadSlowmode.GetValueOrDefault();
    public SortOrderType? DefaultSortOrder => _jsonModel.DefaultSortOrder;
    public ForumLayoutType DefaultForumLayout => _jsonModel.DefaultForumLayout.GetValueOrDefault();

    #region Channel
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task<ForumGuildThread> CreateThreadAsync(ulong channelId, ForumGuildThreadProperties threadProperties, RequestProperties? properties = null) => _client.CreateForumGuildThreadAsync(channelId, threadProperties, properties);
    public Task ModifyPermissionsAsync(PermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(ulong overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    #endregion
}
