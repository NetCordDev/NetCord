namespace NetCord;

public class ChannelPinsUpdateEventArgs
{
    private readonly JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs _jsonEntity;

    public Snowflake? GuildId => _jsonEntity.GuildId;
    public Snowflake ChannelId => _jsonEntity.ChannelId;
    public DateTimeOffset? LastPinTimestamp => _jsonEntity.LastPinTimestamp;

    internal ChannelPinsUpdateEventArgs(JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
