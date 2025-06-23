using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static partial class GatewayHandlerHostExtensions
{
    /// <summary>
    /// Registers all <see cref="IGatewayHandler"/> services to the <see cref="GatewayClient"/>.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance.</param>
    /// <returns>The <see cref="IHost"/> instance.</returns>
    public static IHost UseGatewayHandlers(this IHost host)
    {
        var services = host.Services;

        var client = services.GetRequiredService<GatewayClient>();
        foreach (var handler in services.GetServices<IGatewayHandler>())
        {
            if (handler is IDelegateGatewayHandlerBase delegateHandler)
                RegisterDelegateHandler(client, delegateHandler);
            else
                RegisterClassHandler(client, handler);
        }
        return host;
    }

    /// <summary>
    /// Registers all <see cref="IShardedGatewayHandler"/> services to the <see cref="ShardedGatewayClient"/>.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance.</param>
    /// <returns>The <see cref="IHost"/> instance.</returns>
    public static IHost UseShardedGatewayHandlers(this IHost host)
    {
        var services = host.Services;

        var client = services.GetRequiredService<ShardedGatewayClient>();
        foreach (var handler in services.GetServices<IShardedGatewayHandler>())
        {
            if (handler is IDelegateShardedGatewayHandlerBase delegateHandler)
                RegisterDelegateShardedHandler(client, delegateHandler);
            else
                RegisterClassShardedHandler(client, handler);
        }
        return host;
    }
}
