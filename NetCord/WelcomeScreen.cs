namespace NetCord;

public class WelcomeScreen
{
    private readonly JsonModels.JsonWelcomeScreen _jsonEntity;

    public string? Description => _jsonEntity.Description;

    public IReadOnlyDictionary<DiscordId, WelcomeScreenChannel> WelcomeChannels { get; }

    internal WelcomeScreen(JsonModels.JsonWelcomeScreen jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        WelcomeChannels = jsonEntity.WelcomeChannels.ToDictionary(w => w.ChannelId, w => new WelcomeScreenChannel(w));
    }
}