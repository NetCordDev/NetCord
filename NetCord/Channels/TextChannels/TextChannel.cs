using NetCord.JsonModels;

namespace NetCord;

public abstract class TextChannel : Channel
{
    private protected TextChannel(JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
    {
    }

    public DiscordId? LastMessageId => _jsonEntity.LastMessageId;
    public DateTimeOffset? LastPin => _jsonEntity.LastPin;

    public IAsyncEnumerable<RestMessage> GetMessagesAsync(RequestOptions? options = null) => _client.Rest.Message.GetAsync(Id, options);
    public IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(DiscordId messageId, RequestOptions? options = null) => _client.Rest.Message.GetBeforeAsync(Id, messageId, options);
    public IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(DiscordId messageId, RequestOptions? options = null) => _client.Rest.Message.GetAfterAsync(Id, messageId, options);
    public Task<RestMessage> GetMessageAsync(DiscordId messageId, RequestOptions? options = null) => _client.Rest.Message.GetAsync(Id, messageId, options);
    public Task<RestMessage> SendMessageAsync(Message message, RequestOptions? options = null) => _client.Rest.Message.SendAsync(message, Id, options);
    public Task<RestMessage> SendMessageAsync(string content, RequestOptions? options = null) => _client.Rest.Message.SendAsync(content, Id, options);
}