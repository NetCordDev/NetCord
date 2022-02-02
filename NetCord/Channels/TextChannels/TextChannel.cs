using NetCord.JsonModels;

namespace NetCord;

public abstract class TextChannel : Channel
{
    private protected TextChannel(JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
    }

    public DiscordId? LastMessageId => _jsonEntity.LastMessageId;
    public DateTimeOffset? LastPin => _jsonEntity.LastPin;

    public IAsyncEnumerable<RestMessage> GetMessagesAsync(RequestOptions? options = null) => _client.Message.GetAsync(Id, options);
    public IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(DiscordId messageId, RequestOptions? options = null) => _client.Message.GetBeforeAsync(Id, messageId, options);
    public IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(DiscordId messageId, RequestOptions? options = null) => _client.Message.GetAfterAsync(Id, messageId, options);
    public Task<RestMessage> GetMessageAsync(DiscordId messageId, RequestOptions? options = null) => _client.Message.GetAsync(Id, messageId, options);
    public Task<RestMessage> SendMessageAsync(MessageProperties message, RequestOptions? options = null) => _client.Message.SendAsync(Id, message, options);
}