namespace NetCord;

public class MessageReactionRemoveAllEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs _jsonEntity;

    internal MessageReactionRemoveAllEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public Snowflake ChannelId => _jsonEntity.ChannelId;

    public Snowflake MessageId => _jsonEntity.MessageId;

    public Snowflake? GuildId => _jsonEntity.GuildId;
}
