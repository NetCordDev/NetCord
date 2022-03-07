namespace NetCord;

public class MessageReactionRemoveEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs _jsonEntity;

    internal MessageReactionRemoveEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
        Emoji = new(jsonEntity.Emoji);
    }

    public DiscordId UserId => _jsonEntity.UserId;

    public DiscordId ChannelId => _jsonEntity.ChannelId;

    public DiscordId MessageId => _jsonEntity.MessageId;

    public DiscordId? GuildId => _jsonEntity.GuildId;

    public MessageReactionEmoji Emoji { get; }
}
