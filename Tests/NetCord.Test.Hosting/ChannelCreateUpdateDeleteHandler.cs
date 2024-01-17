using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

[GatewayEvent(nameof(GatewayClient.GuildChannelCreate))]
[GatewayEvent(nameof(GatewayClient.GuildChannelUpdate))]
[GatewayEvent(nameof(GatewayClient.GuildChannelDelete))]
internal class ChannelCreateUpdateDeleteHandler : IGatewayEventHandler<IGuildChannel>
{
    private readonly ILogger<ChannelCreateUpdateDeleteHandler> _logger;

    public ChannelCreateUpdateDeleteHandler(ILogger<ChannelCreateUpdateDeleteHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask HandleAsync(IGuildChannel channel)
    {
        _logger.LogInformation("GuildId: {GuildId}, ChannelId: {ChannelId}", channel.GuildId, channel.Id);

        return default;
    }
}
