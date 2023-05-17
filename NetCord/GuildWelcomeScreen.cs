using System.Collections.Immutable;

namespace NetCord;

public class GuildWelcomeScreen : IJsonModel<JsonModels.JsonGuildWelcomeScreen>
{
    JsonModels.JsonGuildWelcomeScreen IJsonModel<JsonModels.JsonGuildWelcomeScreen>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildWelcomeScreen _jsonModel;

    public string? Description => _jsonModel.Description;

    public ImmutableDictionary<ulong, GuildWelcomeScreenChannel> WelcomeChannels { get; }

    public GuildWelcomeScreen(JsonModels.JsonGuildWelcomeScreen jsonModel)
    {
        _jsonModel = jsonModel;
        WelcomeChannels = jsonModel.WelcomeChannels.ToImmutableDictionary(w => w.ChannelId, w => new GuildWelcomeScreenChannel(w));
    }
}
