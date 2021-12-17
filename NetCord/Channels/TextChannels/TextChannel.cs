using NetCord.JsonModels;

namespace NetCord;

public abstract class TextChannel : Channel
{
    internal TextChannel(JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
    {
    }

    public DiscordId? LastMessageId => _jsonEntity.LastMessageId;
    public DateTimeOffset? LastPin => _jsonEntity.LastPin;

    public IAsyncEnumerable<RestMessage> GetMessagesAsync() => ChannelHelper.GetMessagesAsync(_client, Id);
    public IAsyncEnumerable<RestMessage> GetMessagesBeforeAsync(DiscordId messageId) => ChannelHelper.GetMessagesBeforeAsync(_client, Id, messageId);
    public IAsyncEnumerable<RestMessage> GetMessagesAfterAsync(DiscordId messageId) => ChannelHelper.GetMessagesAfterAsync(_client, Id, messageId);
    public Task<RestMessage> GetMessageAsync(DiscordId messageId) => ChannelHelper.GetMessageAsync(_client, Id, messageId);
    public Task<RestMessage> SendMessageAsync(BuiltMessage message) => ChannelHelper.SendMessageAsync(_client, message, Id);
    public Task<RestMessage> SendMessageAsync(string content) => ChannelHelper.SendMessageAsync(_client, content, Id);
}