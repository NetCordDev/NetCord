namespace NetCord;

public class WelcomeScreen
{
    private readonly JsonModels.JsonWelcomeScreen _jsonEntity;

    public string? Description => _jsonEntity.Description;
    public IReadOnlyDictionary<DiscordId, Channel> WelcomeChannels { get; }

    internal WelcomeScreen(JsonModels.JsonWelcomeScreen jsonEntity, BotClient client)
    {
        _jsonEntity = jsonEntity;
        WelcomeChannels = jsonEntity.WelcomeChannels.ToDictionary(w => w.Id, w => Channel.CreateFromJson(w, client));
    }
}