namespace NetCord;

public class TypingStartEventArgs
{
    private readonly JsonModels.EventArgs.JsonTypingStartEventArgs _jsonEntity;

    internal TypingStartEventArgs(JsonModels.EventArgs.JsonTypingStartEventArgs jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.User != null)
            User = new(jsonEntity.User, _jsonEntity.GuildId.GetValueOrDefault(), client);
    }

    public Snowflake ChannelId => _jsonEntity.ChannelId;

    public Snowflake? GuildId => _jsonEntity.GuildId;

    public Snowflake UserId => _jsonEntity.UserId;

    public DateTimeOffset Timestamp => _jsonEntity.Timestamp;

    public GuildUser? User { get; }
}
