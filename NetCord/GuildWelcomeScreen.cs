namespace NetCord;

public class GuildWelcomeScreen(JsonModels.JsonGuildWelcomeScreen jsonModel) : IJsonModel<JsonModels.JsonGuildWelcomeScreen>
{
    JsonModels.JsonGuildWelcomeScreen IJsonModel<JsonModels.JsonGuildWelcomeScreen>.JsonModel => jsonModel;

    public string? Description => jsonModel.Description;

    public IReadOnlyDictionary<ulong, GuildWelcomeScreenChannel> WelcomeChannels { get; } = jsonModel.WelcomeChannels.ToDictionary(w => w.ChannelId, w => new GuildWelcomeScreenChannel(w));
}
