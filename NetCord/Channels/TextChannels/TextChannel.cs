using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public abstract class TextChannel : Channel
{
    private protected TextChannel(JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public Snowflake? LastMessageId => _jsonModel.LastMessageId;
    public DateTimeOffset? LastPin => _jsonModel.LastPin;

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
    #endregion
}