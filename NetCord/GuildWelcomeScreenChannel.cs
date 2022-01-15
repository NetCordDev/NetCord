namespace NetCord;

public class GuildWelcomeScreenChannel : Entity
{
    private readonly JsonModels.JsonWelcomeScreenChannel _jsonEntity;

    public override DiscordId Id => _jsonEntity.ChannelId;

    public string Description => _jsonEntity.Description;

    public DiscordId? EmojiId => _jsonEntity.EmojiId;

    public string? EmojiName => _jsonEntity.EmojiName;

    internal GuildWelcomeScreenChannel(JsonModels.JsonWelcomeScreenChannel jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}