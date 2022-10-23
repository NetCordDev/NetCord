using NetCord.Rest;

namespace NetCord;

public class TextGuildChannel : TextChannel, IGuildChannel
{
    public ulong? ParentId => _jsonModel.ParentId;
    public string? Topic => _jsonModel.Topic;
    public bool Nsfw => _jsonModel.Nsfw;
    public int Slowmode => _jsonModel.Slowmode.GetValueOrDefault();
    public string Name => _jsonModel.Name!;
    public int Position => _jsonModel.Position.GetValueOrDefault();

    public IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }

    public TextGuildChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    #region Channel
    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task<GuildThread> CreateThreadAsync(ulong messageId, GuildThreadFromMessageProperties threadWithMessageProperties, RequestProperties? properties = null) => _client.CreateGuildThreadAsync(Id, messageId, threadWithMessageProperties, properties);
    public Task<GuildThread> CreateThreadAsync(GuildThreadProperties threadProperties, RequestProperties? properties = null) => _client.CreateGuildThreadAsync(Id, threadProperties, properties);
    public IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(RequestProperties? properties = null) => _client.GetPublicArchivedGuildThreadsAsync(Id, properties);
    public IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsBeforeAsync(DateTimeOffset before, RequestProperties? properties = null) => _client.GetPublicArchivedGuildThreadsBeforeAsync(Id, before, properties);
    public IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsAsync(RequestProperties? properties = null) => _client.GetPrivateArchivedGuildThreadsAsync(Id, properties);
    public IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsBeforeAsync(DateTimeOffset before, RequestProperties? properties = null) => _client.GetPublicArchivedGuildThreadsBeforeAsync(Id, before, properties);
    public IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsAsync(RequestProperties? properties = null) => _client.GetJoinedPrivateArchivedGuildThreadsAsync(Id, properties);
    public IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsBeforeAsync(ulong before, RequestProperties? properties = null) => _client.GetJoinedPrivateArchivedGuildThreadsBeforeAsync(Id, before, properties);
    public Task ModifyPermissionsAsync(ChannelPermissionOverwriteProperties permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(ulong overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);
    #endregion

    #region Webhook
    public Task<Webhook> CreateWebhookAsync(WebhookProperties webhookProperties, RequestProperties? properties = null) => _client.CreateWebhookAsync(Id, webhookProperties, properties);
    public Task<IReadOnlyDictionary<ulong, Webhook>> GetWebhooksAsync(ulong channelId, RequestProperties? properties = null) => _client.GetChannelWebhooksAsync(channelId, properties);
    #endregion
}
