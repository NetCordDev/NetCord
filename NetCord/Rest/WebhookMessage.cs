namespace NetCord.Rest;

public class WebhookMessage : ClientEntity, IJsonModel<JsonModels.JsonMessage>
{
    JsonModels.JsonMessage IJsonModel<JsonModels.JsonMessage>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonMessage _jsonModel;

    public WebhookMessage(JsonModels.JsonMessage jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        Components = jsonModel.Components.SelectOrEmpty(IComponent.CreateFromJson);
        GuildId = jsonModel.GuildId ?? jsonModel.MessageReference?.GuildId;
    }

    public override Snowflake Id => _jsonModel.Id;
    public Snowflake ChannelId => _jsonModel.ChannelId;
    public Snowflake? GuildId { get; }
    public Snowflake? ApplicationId => _jsonModel.ApplicationId;
    public IEnumerable<IComponent> Components { get; }

    public string GetJumpUrl() => $"https://discord.com/channels/{(GuildId.HasValue ? GuildId.GetValueOrDefault() : "@me")}/{ChannelId}/{Id}";

    public string GetJumpUrl(Snowflake? guildId = null) => $"https://discord.com/channels/{(guildId.HasValue ? guildId.GetValueOrDefault() : "@me")}/{ChannelId}/{Id}";

    #region Channel
    public Task<RestMessage> CrosspostAsync(RequestProperties? properties = null) => _client.CrosspostMessageAsync(ChannelId, Id, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteMessageAsync(ChannelId, Id, properties);
    public Task AddReactionAsync(ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.AddMessageReactionAsync(ChannelId, Id, emoji, properties);
    public Task DeleteReactionAsync(ReactionEmojiProperties emoji, Snowflake userId, RequestProperties? properties = null) => _client.DeleteMessageReactionAsync(ChannelId, Id, emoji, userId, properties);
    public IAsyncEnumerable<User> GetReactionsAsync(ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.GetMessageReactionsAsync(ChannelId, Id, emoji, properties);
    public IAsyncEnumerable<User> GetReactionsAfterAsync(ReactionEmojiProperties emoji, Snowflake userId, RequestProperties? properties = null) => _client.GetMessageReactionsAfterAsync(ChannelId, Id, emoji, userId, properties);
    public Task DeleteAllReactionsAsync(ReactionEmojiProperties emoji, RequestProperties? properties = null) => _client.DeleteAllMessageReactionsAsync(ChannelId, Id, emoji, properties);
    public Task DeleteAllReactionsAsync(RequestProperties? properties = null) => _client.DeleteAllMessageReactionsAsync(ChannelId, Id, properties);
    #endregion
}