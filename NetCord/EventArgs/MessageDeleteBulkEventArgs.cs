namespace NetCord;

public class MessageDeleteBulkEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs _jsonEntity;

    internal MessageDeleteBulkEventArgs(JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public IEnumerable<Snowflake> MessageIds => _jsonEntity.MessageIds;

    public Snowflake ChannelId => _jsonEntity.ChannelId;

    public Snowflake? GuildId => _jsonEntity.GuildId;
}
