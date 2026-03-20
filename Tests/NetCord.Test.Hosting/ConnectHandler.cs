using Microsoft.Extensions.Logging;

using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

internal class ConnectHandler(ILogger<ConnectHandler> logger) : IConnectGatewayHandler
{
    public ValueTask HandleAsync()
    {
        logger.LogInformation("Connect received");

        return default;
    }
}
