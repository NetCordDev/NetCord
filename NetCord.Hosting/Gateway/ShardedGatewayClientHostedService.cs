using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

#pragma warning disable CA2254 // Template should be a static expression

internal class ShardedGatewayClientHostedService : IHostedService
{
    private readonly ShardedGatewayClient _client;
    private readonly ILogger<ShardedGatewayClient> _logger;

    public ShardedGatewayClientHostedService(ShardedGatewayClient client, ILogger<ShardedGatewayClient> logger)
    {
        (_client = client).Log += LogAsync;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _client.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _client.CloseAsync();
    }

    private ValueTask LogAsync(GatewayClient client, LogMessage message)
    {
        _logger.Log((LogLevel)message.Severity, new(client.Shard.GetValueOrDefault().Id), message.Exception, message.Message);
        return default;
    }
}
