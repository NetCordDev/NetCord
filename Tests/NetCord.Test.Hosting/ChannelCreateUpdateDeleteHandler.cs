using Microsoft.Extensions.Logging;

using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

internal class ChannelCreateUpdateDeleteHandler(ILogger<ChannelCreateUpdateDeleteHandler> logger) : IGuildChannelCreateGatewayHandler, IGuildChannelUpdateGatewayHandler, IGuildChannelDeleteGatewayHandler
{
    public ValueTask HandleAsync(IGuildChannel channel)
    {
        logger.LogInformation("GuildId: {GuildId}, ChannelId: {ChannelId}", channel.GuildId, channel.Id);

        return default;
    }
}
