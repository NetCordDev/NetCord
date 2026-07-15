using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

internal partial class ShardedGatewayClientHostedService(IServiceProvider services) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var client = services.GetRequiredService<ShardedGatewayClient>();

        foreach (var handlerMetadata in services.GetServices<ShardedGatewayHandlerMetadata>())
        {
            if (handlerMetadata is ClassShardedGatewayHandlerMetadata classHandlerMetadata)
                RegisterClassShardedHandler(services, client, classHandlerMetadata);
            else
                RegisterDelegateShardedHandler(services, client, (DelegateShardedGatewayHandlerMetadata)handlerMetadata);
        }

        var options = services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>().Value;

        return options.AutoStartStop.GetValueOrDefault(true)
            ? client.StartAsync(cancellationToken: cancellationToken).AsTask()
            : Task.CompletedTask;
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
