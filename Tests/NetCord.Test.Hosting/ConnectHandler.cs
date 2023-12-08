using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

[GatewayEvent(nameof(GatewayClient.Connect))]
internal class ConnectHandler : IGatewayEventHandler
{
    private readonly ILogger<ConnectHandler> _logger;

    public ConnectHandler(ILogger<ConnectHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask HandleAsync()
    {
        _logger.LogInformation("Connect received");

        return default;
    }
}
