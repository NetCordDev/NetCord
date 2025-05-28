using Microsoft.Extensions.Hosting;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

internal class GatewayClientHostedService(GatewayClient client) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return client.StartAsync(cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return client.CloseAsync(cancellationToken: cancellationToken);
    }
}
