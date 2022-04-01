namespace NetCord;

public class MessageDeleteEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageDeleteEventArgs _jsonEntity;

    internal MessageDeleteEventArgs(JsonModels.EventArgs.JsonMessageDeleteEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public Snowflake MessageId => _jsonEntity.MessageId;

    public Snowflake ChannelId => _jsonEntity.ChannelId;

    public Snowflake? GuildId => _jsonEntity.GuildId;
}
