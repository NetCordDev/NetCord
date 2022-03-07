namespace NetCord;

public class MessageDeleteEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageDeleteEventArgs _jsonEntity;

    internal MessageDeleteEventArgs(JsonModels.EventArgs.JsonMessageDeleteEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public DiscordId MessageId => _jsonEntity.MessageId;

    public DiscordId ChannelId => _jsonEntity.ChannelId;

    public DiscordId? GuildId => _jsonEntity.GuildId;
}
