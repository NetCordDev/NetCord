namespace NetCord;

public class MessageReactionRemoveEmojiEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs _jsonEntity;

    internal MessageReactionRemoveEmojiEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Emoji = new(jsonEntity.Emoji);
    }

    public DiscordId ChannelId => _jsonEntity.ChannelId;

    public DiscordId? GuildId => _jsonEntity.GuildId;

    public DiscordId MessageId => _jsonEntity.MessageId;

    public MessageReactionEmoji Emoji { get; }
}
