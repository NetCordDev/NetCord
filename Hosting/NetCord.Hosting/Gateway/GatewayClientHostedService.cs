using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

internal partial class GatewayClientHostedService(IServiceProvider services) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var client = services.GetRequiredService<GatewayClient>();

        foreach (var handler in services.GetServices<IGatewayHandler>())
        {
            if (handler is IDelegateGatewayHandlerBase delegateHandler)
                RegisterDelegateHandler(client, delegateHandler);
            else
                RegisterClassHandler(client, handler);
        }

        return client.StartAsync(cancellationToken: cancellationToken).AsTask();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var client = services.GetRequiredService<GatewayClient>();

        return client.CloseAsync(cancellationToken: cancellationToken).AsTask();
    }
}
