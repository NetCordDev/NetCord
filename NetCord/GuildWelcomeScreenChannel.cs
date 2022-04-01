namespace NetCord;

public class GuildWelcomeScreenChannel : Entity
{
    private readonly JsonModels.JsonWelcomeScreenChannel _jsonEntity;

    public override Snowflake Id => _jsonEntity.ChannelId;

    public string Description => _jsonEntity.Description;

    public Snowflake? EmojiId => _jsonEntity.EmojiId;

    public string? EmojiName => _jsonEntity.EmojiName;

    internal GuildWelcomeScreenChannel(JsonModels.JsonWelcomeScreenChannel jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}