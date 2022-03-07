namespace NetCord;

public class MessageReactionAddEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageReactionAddEventArgs _jsonEntity;

    internal MessageReactionAddEventArgs(JsonModels.EventArgs.JsonMessageReactionAddEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.User != null)
            User = new(jsonEntity.User, jsonEntity.GuildId.GetValueOrDefault(), client);
        Emoji = new(jsonEntity.Emoji);
    }

    public DiscordId UserId => _jsonEntity.UserId;

    public DiscordId ChannelId => _jsonEntity.ChannelId;

    public DiscordId MessageId => _jsonEntity.MessageId;

    public DiscordId? GuildId => _jsonEntity.GuildId;

    public GuildUser? User { get; }

    public MessageReactionEmoji Emoji { get; }
}
