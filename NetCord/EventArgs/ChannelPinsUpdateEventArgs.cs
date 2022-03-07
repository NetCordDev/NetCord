namespace NetCord;

public class ChannelPinsUpdateEventArgs
{
    private readonly JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs _jsonEntity;

    public DiscordId? GuildId => _jsonEntity.GuildId;
    public DiscordId ChannelId => _jsonEntity.ChannelId;
    public DateTimeOffset? LastPinTimestamp => _jsonEntity.LastPinTimestamp;

    internal ChannelPinsUpdateEventArgs(JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
