using NetCord.JsonModels;

namespace NetCord;

public class GuildWelcomeScreen(JsonGuildWelcomeScreen jsonModel)
{
    public string? Description => jsonModel.Description;

    public IReadOnlyDictionary<ulong, GuildWelcomeScreenChannel> WelcomeChannels { get; } = jsonModel.WelcomeChannels.ToDictionary(w => w.ChannelId, w => new GuildWelcomeScreenChannel(w));
}
