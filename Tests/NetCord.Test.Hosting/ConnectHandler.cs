using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

[GatewayEvent(nameof(GatewayClient.Connect))]
internal class ConnectHandler(ILogger<ConnectHandler> logger) : IGatewayEventHandler
{
    public ValueTask HandleAsync()
    {
        logger.LogInformation("Connect received");

        return default;
    }
}
