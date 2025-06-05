using Microsoft.Extensions.Hosting;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

internal class ShardedGatewayClientHostedService(ShardedGatewayClient client) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return client.StartAsync(cancellationToken: cancellationToken).AsTask();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return client.CloseAsync(cancellationToken: cancellationToken).AsTask();
    }
}
