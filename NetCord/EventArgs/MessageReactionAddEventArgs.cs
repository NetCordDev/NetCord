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

    public Snowflake UserId => _jsonEntity.UserId;

    public Snowflake ChannelId => _jsonEntity.ChannelId;

    public Snowflake MessageId => _jsonEntity.MessageId;

    public Snowflake? GuildId => _jsonEntity.GuildId;

    public GuildUser? User { get; }

    public MessageReactionEmoji Emoji { get; }
}
