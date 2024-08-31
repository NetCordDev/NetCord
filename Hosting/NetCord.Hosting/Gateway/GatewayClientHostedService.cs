using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

#pragma warning disable CA2254 // Template should be a static expression

internal class GatewayClientHostedService : IHostedService
{
    private readonly GatewayClient _client;
    private readonly ILogger<GatewayClient> _logger;

    public GatewayClientHostedService(GatewayClient client, ILogger<GatewayClient> logger)
    {
        (_client = client).Log += LogAsync;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _client.StartAsync(cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _client.CloseAsync(cancellationToken: cancellationToken);
    }

    private ValueTask LogAsync(LogMessage message)
    {
        _logger.Log((LogLevel)message.Severity, message.Exception, message.Message);
        return default;
    }
}
