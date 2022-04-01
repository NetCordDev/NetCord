namespace NetCord;

public class MessageReactionRemoveEmojiEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs _jsonEntity;

    internal MessageReactionRemoveEmojiEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Emoji = new(jsonEntity.Emoji);
    }

    public Snowflake ChannelId => _jsonEntity.ChannelId;

    public Snowflake? GuildId => _jsonEntity.GuildId;

    public Snowflake MessageId => _jsonEntity.MessageId;

    public MessageReactionEmoji Emoji { get; }
}
