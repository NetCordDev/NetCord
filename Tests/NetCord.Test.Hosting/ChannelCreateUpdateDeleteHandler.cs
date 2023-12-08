using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

[GatewayEvent(nameof(GatewayClient.GuildChannelCreate))]
[GatewayEvent(nameof(GatewayClient.GuildChannelUpdate))]
[GatewayEvent(nameof(GatewayClient.GuildChannelDelete))]
internal class ChannelCreateUpdateDeleteHandler : IGatewayEventHandler<GuildChannelEventArgs>
{
    private readonly ILogger<ChannelCreateUpdateDeleteHandler> _logger;

    public ChannelCreateUpdateDeleteHandler(ILogger<ChannelCreateUpdateDeleteHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask HandleAsync(GuildChannelEventArgs args)
    {
        _logger.LogInformation("GuildId: {GuildId}, ChannelId: {ChannelId}", args.GuildId, args.Channel.Id);

        return default;
    }
}
