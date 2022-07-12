namespace NetCord.Rest;

public class FollowedChannel : ClientEntity, IJsonModel<JsonModels.JsonFollowedChannel>
{
    JsonModels.JsonFollowedChannel IJsonModel<JsonModels.JsonFollowedChannel>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonFollowedChannel _jsonModel;

    public override Snowflake Id => _jsonModel.Id;

    public Snowflake WebhookId => _jsonModel.WebhookId;

    public FollowedChannel(JsonModels.JsonFollowedChannel jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    #region Channel
    public Task TriggerTypingStateAsync(RequestProperties? properties = null) => _client.TriggerTypingStateAsync(Id, properties);
    public Task<IDisposable> EnterTypingStateAsync(RequestProperties? properties = null) => _client.EnterTypingStateAsync(Id, properties);
    public Task<IReadOnlyDictionary<Snowflake, RestMessage>> GetPinnedMessagesAsync(RequestProperties? properties = null) => _client.GetPinnedMessagesAsync(Id, properties);
    public Task PinMessageAsync(Snowflake messageId, RequestProperties? properties = null) => _client.PinMessageAsync(Id, messageId, properties);
    public Task UnpinMessageAsync(Snowflake messageId, RequestProperties? properties = null) => _client.UnpinMessageAsync(Id, messageId, properties);
    public Task<RestMessage> SendMessageAsync(MessageProperties message, RequestProperties? properties = null) => _client.SendMessageAsync(Id, message, properties);
    public Task<RestMessage> ModifyMessageAsync(Snowflake messageId, Action<MessageOptions> action, RequestProperties? properties = null) => _client.ModifyMessageAsync(Id, messageId, action, properties);
    public Task DeleteMessageAsync(Snowflake messageId, RequestProperties? properties = null) => _client.DeleteMessageAsync(Id, messageId, properties);
    public Task<RestMessage> GetMessageAsync(Snowflake messageId, RequestProperties? properties = null) => _client.GetMessageAsync(Id, messageId, properties);
    public IAsyncEnumerable<RestMessage> GetMessagesAsync(RequestProperties? properties = null) => _client.GetMessagesAsync(Id, properties);
    public IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(Snowflake messageId, RequestProperties? properties = null) => _client.GetMessagesBeforeAsync(Id, messageId, properties);
    public IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(Snowflake messageId, RequestProperties? properties = null) => _client.GetMessagesAfterAsync(Id, messageId, properties);
    public Task AddMessageReactionAsync(Snowflake messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.AddMessageReactionAsync(Id, messageId, emoji, properties);
    public Task DeleteMessageReactionAsync(Snowflake messageId, ReactionEmojiProperties emoji, Snowflake userId, RequestProperties? properties = null) => _client.DeleteMessageReactionAsync(Id, messageId, emoji, userId, properties);
    public IAsyncEnumerable<User> GetMessageReactionsAsync(Snowflake messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.GetMessageReactionsAsync(Id, messageId, emoji, properties);
    public IAsyncEnumerable<User> GetMessageReactionsAfterAsync(Snowflake messageId, ReactionEmojiProperties emoji, Snowflake userId, RequestProperties? properties = null) => _client.GetMessageReactionsAfterAsync(Id, messageId, emoji, userId, properties);
    public Task DeleteAllMessageReactionsAsync(Snowflake messageId, ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.DeleteAllMessageReactionsAsync(Id, messageId, emoji, properties);
    public Task DeleteAllMessageReactionsAsync(Snowflake messageId, RequestProperties? properties = null) => _client.DeleteAllMessageReactionsAsync(Id, messageId, properties);

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? properties = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, properties).ConfigureAwait(false);
    public Task<GuildThread> CreateThreadAsync(Snowflake messageId, ThreadWithMessageProperties threadWithMessageProperties, RequestProperties? properties = null) => _client.CreateGuildThreadAsync(Id, messageId, threadWithMessageProperties, properties);
    public Task<GuildThread> CreateThreadAsync(ThreadProperties threadProperties, RequestProperties? properties = null) => _client.CreateGuildThreadAsync(Id, threadProperties, properties);
    public IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsAsync(RequestProperties? properties = null) => _client.GetPublicArchivedGuildThreadsAsync(Id, properties);
    public IAsyncEnumerable<GuildThread> GetPublicArchivedGuildThreadsBeforeAsync(DateTimeOffset before, RequestProperties? properties = null) => _client.GetPublicArchivedGuildThreadsBeforeAsync(Id, before, properties);
    public IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsAsync(RequestProperties? properties = null) => _client.GetPrivateArchivedGuildThreadsAsync(Id, properties);
    public IAsyncEnumerable<GuildThread> GetPrivateArchivedGuildThreadsBeforeAsync(DateTimeOffset before, RequestProperties? properties = null) => _client.GetPublicArchivedGuildThreadsBeforeAsync(Id, before, properties);
    public IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsAsync(RequestProperties? properties = null) => _client.GetJoinedPrivateArchivedGuildThreadsAsync(Id, properties);
    public IAsyncEnumerable<GuildThread> GetJoinedPrivateArchivedGuildThreadsBeforeAsync(Snowflake before, RequestProperties? properties = null) => _client.GetJoinedPrivateArchivedGuildThreadsBeforeAsync(Id, before, properties);
    public Task ModifyPermissionsAsync(ChannelPermissionOverwrite permissionOverwrite, RequestProperties? properties = null) => _client.ModifyGuildChannelPermissionsAsync(Id, permissionOverwrite, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildChannelInvitesAsync(Id, properties);
    public Task<RestGuildInvite> CreateInviteAsync(GuildInviteProperties? guildInviteProperties = null, RequestProperties? properties = null) => _client.CreateGuildChannelInviteAsync(Id, guildInviteProperties, properties);
    public Task DeletePermissionAsync(Snowflake overwriteId, RequestProperties? properties = null) => _client.DeleteGuildChannelPermissionAsync(Id, overwriteId, properties);

    public Task<RestMessage> CrosspostMessageAsync(Snowflake messageId, RequestProperties? properties = null) => _client.CrosspostMessageAsync(Id, messageId, properties);
    public Task<FollowedChannel> FollowAsync(Snowflake targetChannelId, RequestProperties? properties = null) => _client.FollowNewsGuildChannelAsync(Id, targetChannelId, properties);
    #endregion

    #region Webhook
    public Task<Webhook> CreateWebhookAsync(WebhookProperties webhookProperties, RequestProperties? properties = null) => _client.CreateWebhookAsync(Id, webhookProperties, properties);
    public Task<IReadOnlyDictionary<Snowflake, Webhook>> GetWebhooksAsync(Snowflake channelId, RequestProperties? properties = null) => _client.GetChannelWebhooksAsync(channelId, properties);
    #endregion
}