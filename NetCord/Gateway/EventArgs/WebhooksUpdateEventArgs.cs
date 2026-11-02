using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class WebhooksUpdateEventArgs(JsonWebhooksUpdateEventArgs jsonModel)
{
    public ulong GuildId { get; } = jsonModel.GuildId;

    public ulong ChannelId { get; } = jsonModel.ChannelId;
}
