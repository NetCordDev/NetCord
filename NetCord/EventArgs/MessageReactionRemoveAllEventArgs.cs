namespace NetCord;

public class MessageReactionRemoveAllEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs _jsonEntity;

    internal MessageReactionRemoveAllEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public DiscordId ChannelId => _jsonEntity.ChannelId;

    public DiscordId MessageId => _jsonEntity.MessageId;

    public DiscordId? GuildId => _jsonEntity.GuildId;
}
