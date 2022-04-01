namespace NetCord;

public class MessageReactionRemoveEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs _jsonEntity;

    internal MessageReactionRemoveEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Emoji = new(jsonEntity.Emoji);
    }

    public Snowflake UserId => _jsonEntity.UserId;

    public Snowflake ChannelId => _jsonEntity.ChannelId;

    public Snowflake MessageId => _jsonEntity.MessageId;

    public Snowflake? GuildId => _jsonEntity.GuildId;

    public MessageReactionEmoji Emoji { get; }
}
