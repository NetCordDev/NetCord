using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static class GatewayEventHandlerHostExtensions
{
    private const string CheckId = "IL2075:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.";
    private const string Justification = "'DynamicDependencyAttribute' is applied to preserve the methods.";

    public static IHost UseGatewayEventHandlers(this IHost host)
    {
        var services = host.Services;

        var client = services.GetRequiredService<GatewayClient>();
        RegisterGatewayClientHandlers(client, services);
        return host;
    }

    public static IHost UseShardedGatewayEventHandlers(this IHost host)
    {
        var services = host.Services;

        var shardedClient = services.GetRequiredService<ShardedGatewayClient>();
        RegisterShardedGatewayClientHandlers(shardedClient, services);
        return host;
    }

    [DynamicDependency(DAMT.PublicMethods, typeof(IGatewayEventHandler<>))]
    [UnconditionalSuppressMessage("Trimming", CheckId, Justification = Justification)]
    private static void RegisterGatewayClientHandlers(GatewayClient client, IServiceProvider services)
    {
        var events = typeof(GatewayClient).GetEvents().ToDictionary(e => e.Name);

        foreach (var handler in services.GetServices<IGatewayEventHandlerBase>())
        {
            foreach (var eventName in handler.GetEvents())
            {
                if (!events.TryGetValue(eventName, out var @event))
                    throw new InvalidOperationException($"Event '{eventName}' does not exist for '{nameof(GatewayClient)}'.");

                var eventType = @event.EventHandlerType!;
                var eventArguments = eventType.GenericTypeArguments;

                Delegate @delegate;

                if (eventArguments.Length is 1)
                {
                    if (handler is not IGatewayEventHandler gatewayEventHandler)
                        throw new InvalidOperationException($"Handler '{handler.GetType()}' does not implement the required interface '{typeof(IGatewayEventHandler)}' for event '{eventName}'.");

                    @delegate = gatewayEventHandler.HandleAsync;
                }
                else
                {
                    var handlerType = handler.GetType();
                    var firstArgument = eventArguments[0];
                    var @interface = handlerType.GetInterfaces().FirstOrDefault(i => i.IsGenericType &&
                                                                                     i.GetGenericTypeDefinition() == typeof(IGatewayEventHandler<>) &&
                                                                                     i.GenericTypeArguments[0] == firstArgument)
                        ?? throw new InvalidOperationException($"Handler '{handlerType}' does not implement the required interface '{nameof(IGatewayEventHandler)}`1[{firstArgument}]' for event '{eventName}'.");

                    var method = @interface.GetMethod(nameof(IGatewayEventHandler.HandleAsync))!;
                    @delegate = method.CreateDelegate(eventType, handler);
                }

                @event.AddEventHandler(client, @delegate);
            }
        }
    }

    [DynamicDependency(DAMT.PublicMethods, typeof(IShardedGatewayEventHandler<>))]
    [UnconditionalSuppressMessage("Trimming", CheckId, Justification = Justification)]
    private static void RegisterShardedGatewayClientHandlers(ShardedGatewayClient client, IServiceProvider services)
    {
        var events = typeof(ShardedGatewayClient).GetEvents().ToDictionary(e => e.Name);

        foreach (var handler in services.GetServices<IShardedGatewayEventHandlerBase>())
        {
            foreach (var eventName in handler.GetEvents())
            {
                if (!events.TryGetValue(eventName, out var @event))
                    throw new InvalidOperationException($"Event '{eventName}' does not exist for '{nameof(GatewayClient)}'.");

                var eventType = @event.EventHandlerType!;
                var eventArguments = eventType.GenericTypeArguments;

                Delegate @delegate;

                if (eventArguments.Length is 2)
                {
                    if (handler is not IShardedGatewayEventHandler gatewayEventHandler)
                        throw new InvalidOperationException($"Handler '{handler.GetType()}' does not implement the required interface '{typeof(IShardedGatewayEventHandler)}' for event '{eventName}'.");

                    @delegate = gatewayEventHandler.HandleAsync;
                }
                else
                {
                    var handlerType = handler.GetType();
                    var firstArgument = eventArguments[1];
                    var @interface = handlerType.GetInterfaces().FirstOrDefault(i => i.IsGenericType &&
                                                                                     i.GetGenericTypeDefinition() == typeof(IShardedGatewayEventHandler<>) &&
                                                                                     i.GenericTypeArguments[0] == firstArgument)
                        ?? throw new InvalidOperationException($"Handler '{handlerType}' does not implement the required interface '{nameof(IShardedGatewayEventHandler)}`1[{firstArgument}]' for event '{eventName}'.");

                    var method = @interface.GetMethod(nameof(IShardedGatewayEventHandler.HandleAsync))!;
                    @delegate = method.CreateDelegate(eventType, handler);
                }

                @event.AddEventHandler(client, @delegate);
            }
        }
    }
}
