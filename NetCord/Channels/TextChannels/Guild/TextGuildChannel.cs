using NetCord.Rest;

namespace NetCord;

public class TextGuildChannel : TextChannel, IGuildChannel
{
    public TextGuildChannel(JsonModels.JsonChannel jsonModel, ulong guildId, RestClient client) : base(jsonModel, client)
    {
        GuildId = guildId;
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public ulong GuildId { get; }
    public int? Position => _jsonModel.Position;
    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
    public string Name => _jsonModel.Name!;
    public string? Topic => _jsonModel.Topic;
    public bool Nsfw => _jsonModel.Nsfw.GetValueOrDefault();
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();
    public ulong? ParentId => _jsonModel.ParentId;
    public int? DefaultAutoArchiveDuration => _jsonModel.DefaultAutoArchiveDuration;
    public int DefaultThreadSlowmode => _jsonModel.DefaultThreadSlowmode.GetValueOrDefault();

    #region Channel
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task ModifyPermissionsAsync(PermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(ulong overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    public Task<GuildThread> CreateThreadAsync(ulong messageId, GuildThreadFromMessageProperties threadWithMessageProperties, RequestProperties? properties = null) => _client.CreateGuildThreadAsync(Id, messageId, threadWithMessageProperties, properties);
    public Task<GuildThread> CreateThreadAsync(GuildThreadProperties threadProperties, RequestProperties? properties = null) => _client.CreateGuildThreadAsync(Id, threadProperties, properties);
    public IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(PaginationProperties<DateTimeOffset>? paginationProperties = null, RequestProperties? properties = null) => _client.GetPublicArchivedGuildThreadsAsync(Id, paginationProperties, properties);
    public IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsAsync(PaginationProperties<DateTimeOffset>? paginationProperties = null, RequestProperties? properties = null) => _client.GetPrivateArchivedGuildThreadsAsync(Id, paginationProperties, properties);
    public IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsAsync(PaginationProperties<ulong>? paginationProperties = null, RequestProperties? properties = null) => _client.GetJoinedPrivateArchivedGuildThreadsAsync(Id, paginationProperties, properties);
    #endregion

    #region Webhook
    public Task<Webhook> CreateWebhookAsync(WebhookProperties webhookProperties, RequestProperties? properties = null) => _client.CreateWebhookAsync(Id, webhookProperties, properties);
    public Task<IReadOnlyDictionary<ulong, Webhook>> GetWebhooksAsync(ulong channelId, RequestProperties? properties = null) => _client.GetChannelWebhooksAsync(channelId, properties);
    #endregion
}
