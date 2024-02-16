using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

[GatewayEvent(nameof(GatewayClient.GuildChannelCreate))]
[GatewayEvent(nameof(GatewayClient.GuildChannelUpdate))]
[GatewayEvent(nameof(GatewayClient.GuildChannelDelete))]
internal class ChannelCreateUpdateDeleteHandler(ILogger<ChannelCreateUpdateDeleteHandler> logger) : IGatewayEventHandler<IGuildChannel>
{
    public ValueTask HandleAsync(IGuildChannel channel)
    {
        logger.LogInformation("GuildId: {GuildId}, ChannelId: {ChannelId}", channel.GuildId, channel.Id);

        return default;
    }
}
