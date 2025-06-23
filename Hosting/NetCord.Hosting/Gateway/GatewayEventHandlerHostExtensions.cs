using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static partial class GatewayEventHandlerHostExtensions
{
    /// <summary>
    /// Registers all <see cref="IGatewayEventHandler"/> services to the <see cref="GatewayClient"/>.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance.</param>
    /// <returns>The <see cref="IHost"/> instance.</returns>
    public static IHost UseGatewayEventHandlers(this IHost host)
    {
        var services = host.Services;

        var client = services.GetRequiredService<GatewayClient>();
        foreach (var handler in services.GetServices<IGatewayEventHandler>())
        {
            if (handler is IDelegateGatewayEventHandlerBase delegateHandler)
                RegisterDelegateHandler(client, delegateHandler);
            else
                RegisterClassHandler(client, handler);
        }
        return host;
    }

    /// <summary>
    /// Registers all <see cref="IShardedGatewayEventHandler"/> services to the <see cref="ShardedGatewayClient"/>.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance.</param>
    /// <returns>The <see cref="IHost"/> instance.</returns>
    public static IHost UseShardedGatewayEventHandlers(this IHost host)
    {
        var services = host.Services;

        var client = services.GetRequiredService<ShardedGatewayClient>();
        foreach (var handler in services.GetServices<IShardedGatewayEventHandler>())
        {
            if (handler is IDelegateShardedGatewayEventHandlerBase delegateHandler)
                RegisterDelegateShardedHandler(client, delegateHandler);
            else
                RegisterClassShardedHandler(client, handler);
        }
        return host;
    }
}
