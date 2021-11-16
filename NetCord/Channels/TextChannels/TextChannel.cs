using NetCord.JsonModels;

namespace NetCord;

public abstract class TextChannel : Channel
{
    internal TextChannel(JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
    {
    }

    public DiscordId? LastMessageId => _jsonEntity.LastMessageId;
    public DateTimeOffset? LastPin => _jsonEntity.LastPin;

    public IAsyncEnumerable<Message> GetMessagesAsync() => ChannelHelper.GetMessagesAsync(_client, Id);
    public IAsyncEnumerable<Message> GetMessagesBeforeAsync(DiscordId messageId) => ChannelHelper.GetMessagesBeforeAsync(_client, Id, messageId);
    public IAsyncEnumerable<Message> GetMessagesAfterAsync(DiscordId messageId) => ChannelHelper.GetMessagesAfterAsync(_client, Id, messageId);
    public Task<Message> GetMessageAsync(DiscordId messageId) => ChannelHelper.GetMessageAsync(_client, Id, messageId);
    public Task<Message> SendMessageAsync(BuiltMessage message) => ChannelHelper.SendMessageAsync(_client, message, Id);
    public Task<Message> SendMessageAsync(string content) => ChannelHelper.SendMessageAsync(_client, content, Id);
}