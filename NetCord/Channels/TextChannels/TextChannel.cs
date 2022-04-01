using NetCord.JsonModels;

namespace NetCord;

public abstract class TextChannel : Channel
{
    private protected TextChannel(JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
    }

    public Snowflake? LastMessageId => _jsonEntity.LastMessageId;
    public DateTimeOffset? LastPin => _jsonEntity.LastPin;

    public IAsyncEnumerable<RestMessage> GetMessagesAsync(RequestProperties? options = null) => _client.GetMessagesAsync(Id, options);
    public IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(Snowflake messageId, RequestProperties? options = null) => _client.GetMessagesBeforeAsync(Id, messageId, options);
    public IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(Snowflake messageId, RequestProperties? options = null) => _client.GetMessagesAfterAsync(Id, messageId, options);
    public Task<RestMessage> GetMessageAsync(Snowflake messageId, RequestProperties? options = null) => _client.GetMessageAsync(Id, messageId, options);
    public Task<RestMessage> SendMessageAsync(MessageProperties message, RequestProperties? options = null) => _client.SendMessageAsync(Id, message, options);
}