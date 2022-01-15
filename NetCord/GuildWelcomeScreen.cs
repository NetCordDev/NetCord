namespace NetCord;

public class GuildWelcomeScreen
{
    private readonly JsonModels.JsonWelcomeScreen _jsonEntity;

    public string? Description => _jsonEntity.Description;

    public IReadOnlyDictionary<DiscordId, GuildWelcomeScreenChannel> WelcomeChannels { get; }

    internal GuildWelcomeScreen(JsonModels.JsonWelcomeScreen jsonEntity)
    {
        _jsonEntity = jsonEntity;
        WelcomeChannels = jsonEntity.WelcomeChannels.ToDictionary(w => w.ChannelId, w => new GuildWelcomeScreenChannel(w));
    }
}