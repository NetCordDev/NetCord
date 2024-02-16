using System.Collections.Immutable;

namespace NetCord;

public class GuildWelcomeScreen(JsonModels.JsonGuildWelcomeScreen jsonModel) : IJsonModel<JsonModels.JsonGuildWelcomeScreen>
{
    JsonModels.JsonGuildWelcomeScreen IJsonModel<JsonModels.JsonGuildWelcomeScreen>.JsonModel => jsonModel;

    public string? Description => jsonModel.Description;

    public ImmutableDictionary<ulong, GuildWelcomeScreenChannel> WelcomeChannels { get; } = jsonModel.WelcomeChannels.ToImmutableDictionary(w => w.ChannelId, w => new GuildWelcomeScreenChannel(w));
}
