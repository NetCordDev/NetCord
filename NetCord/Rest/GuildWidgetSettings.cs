using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildWidgetSettings(JsonGuildWidgetSettings jsonModel)
{
    public bool Enabled { get; } = jsonModel.Enabled;

    public ulong? ChannelId { get; } = jsonModel.ChannelId;
}
