using System.Collections.Immutable;

namespace NetCord;

public class GuildWelcomeScreen
{
    private readonly JsonModels.JsonWelcomeScreen _jsonEntity;

    public string? Description => _jsonEntity.Description;

    public ImmutableDictionary<Snowflake, GuildWelcomeScreenChannel> WelcomeChannels { get; }

    internal GuildWelcomeScreen(JsonModels.JsonWelcomeScreen jsonEntity)
    {
        _jsonEntity = jsonEntity;
        WelcomeChannels = jsonEntity.WelcomeChannels.ToImmutableDictionary(w => w.ChannelId, w => new GuildWelcomeScreenChannel(w));
    }
}