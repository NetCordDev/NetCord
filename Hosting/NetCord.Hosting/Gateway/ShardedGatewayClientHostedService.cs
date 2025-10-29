using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

internal partial class ShardedGatewayClientHostedService(IServiceProvider services) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var options = services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>().Value;

        if (!options.AutoStartStop.GetValueOrDefault(true))
            return Task.CompletedTask;

        var client = services.GetRequiredService<ShardedGatewayClient>();

        foreach (var handler in services.GetServices<IShardedGatewayHandler>())
        {
            if (handler is IDelegateShardedGatewayHandlerBase delegateHandler)
                RegisterDelegateShardedHandler(client, delegateHandler);
            else
                RegisterClassShardedHandler(client, handler);
        }

        return client.StartAsync(cancellationToken: cancellationToken).AsTask();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var options = services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>().Value;

        if (!options.AutoStartStop.GetValueOrDefault(true))
            return Task.CompletedTask;

        var client = services.GetRequiredService<ShardedGatewayClient>();

        return client.CloseAsync(cancellationToken: cancellationToken).AsTask();
    }
}
