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

    public IAsyncEnumerable<RestMessage> GetMessagesAsync(RequestProperties? properties = null) => _client.GetMessagesAsync(Id, properties);
    public IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(Snowflake messageId, RequestProperties? properties = null) => _client.GetMessagesBeforeAsync(Id, messageId, properties);
    public IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(Snowflake messageId, RequestProperties? properties = null) => _client.GetMessagesAfterAsync(Id, messageId, properties);
    public Task<RestMessage> GetMessageAsync(Snowflake messageId, RequestProperties? properties = null) => _client.GetMessageAsync(Id, messageId, properties);
    public Task<RestMessage> SendMessageAsync(MessageProperties message, RequestProperties? properties = null) => _client.SendMessageAsync(Id, message, properties);
}