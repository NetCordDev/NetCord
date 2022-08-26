using System.Collections.Immutable;

namespace NetCord;

public class GuildWelcomeScreen : IJsonModel<JsonModels.JsonWelcomeScreen>
{
    JsonModels.JsonWelcomeScreen IJsonModel<JsonModels.JsonWelcomeScreen>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonWelcomeScreen _jsonModel;

    public string? Description => _jsonModel.Description;

    public ImmutableDictionary<Snowflake, GuildWelcomeScreenChannel> WelcomeChannels { get; }

    public GuildWelcomeScreen(JsonModels.JsonWelcomeScreen jsonModel)
    {
        _jsonModel = jsonModel;
        WelcomeChannels = jsonModel.WelcomeChannels.ToImmutableDictionary(w => w.ChannelId, w => new GuildWelcomeScreenChannel(w));
    }
}
