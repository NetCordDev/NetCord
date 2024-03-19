using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static class GatewayEventHandlerHostExtensions
{
    public static IHost UseGatewayEventHandlers(this IHost host)
    {
        var services = host.Services;

        var client = services.GetService<GatewayClient>();
        if (client is not null)
        {
            RegisterGatewayClientHandlers(client, services);

            var shardedClient = services.GetService<ShardedGatewayClient>();
            if (shardedClient is not null)
                RegisterShardedGatewayClientHandlers(shardedClient, services);
            return host;
        }
        else
        {
            var shardedClient = services.GetService<ShardedGatewayClient>();
            if (shardedClient is not null)
            {
                RegisterShardedGatewayClientHandlers(shardedClient, services);
                return host;
            }
        }

        throw new InvalidOperationException($"None of '{nameof(GatewayClient)}' and '{nameof(ShardedGatewayClient)}' have been registered.");
    }

    private static void RegisterGatewayClientHandlers(GatewayClient client, IServiceProvider services)
    {
        var events = typeof(GatewayClient).GetEvents().ToDictionary(e => e.Name);

        foreach (var handler in services.GetServices<IGatewayEventHandlerBase>())
        {
            foreach (var eventName in handler.GetEvents())
            {
                if (!events.TryGetValue(eventName, out var @event))
                    throw new InvalidOperationException($"Event '{eventName}' does not exist for '{nameof(GatewayClient)}'.");

                var eventTypeArguments = @event.EventHandlerType!.GenericTypeArguments;
                if (handler is IGatewayEventHandler gatewayEventHandler)
                {
                    if (eventTypeArguments.Length != 1)
                        throw new InvalidOperationException($"Handler '{handler.GetType()}' does not match the type arguments of the '{@event.Name}' event.");

                    @event.AddEventHandler(client, gatewayEventHandler.HandleAsync);
                }
                else
                {
                    var handlerType = handler.GetType();

                    if (eventTypeArguments.Length != 2)
                        throw new InvalidOperationException($"Handler '{handlerType}' does not match the type arguments of the '{@event.Name}' event.");

                    var parameterType = eventTypeArguments[0];
                    var @interface = typeof(IGatewayEventHandler<>).MakeGenericType(parameterType);

                    if (!handlerType.IsAssignableTo(@interface))
                        throw new InvalidOperationException($"Handler '{handlerType}' does not implement the required interface '{@interface}'.");

                    var method = @interface.GetMethod(nameof(IGatewayEventHandler.HandleAsync))!;

                    @event.AddEventHandler(client, method.CreateDelegate(typeof(Func<,>).MakeGenericType(parameterType, typeof(ValueTask)), handler));
                }
            }
        }
    }

    private static void RegisterShardedGatewayClientHandlers(ShardedGatewayClient client, IServiceProvider services)
    {
        var events = typeof(ShardedGatewayClient).GetEvents().ToDictionary(e => e.Name);

        foreach (var handler in services.GetServices<IShardedGatewayEventHandlerBase>())
        {
            foreach (var eventName in handler.GetEvents())
            {
                if (!events.TryGetValue(eventName, out var @event))
                    throw new InvalidOperationException($"Event '{eventName}' does not exist for '{nameof(ShardedGatewayClient)}'.");

                var eventTypeArguments = @event.EventHandlerType!.GenericTypeArguments;
                if (handler is IShardedGatewayEventHandler shardedGatewayEventHandler)
                {
                    if (eventTypeArguments.Length != 2)
                        throw new InvalidOperationException($"Handler '{handler.GetType()}' does not match the type arguments of the '{@event.Name}' event.");

                    @event.AddEventHandler(client, shardedGatewayEventHandler.HandleAsync);
                }
                else
                {
                    var handlerType = handler.GetType();

                    if (eventTypeArguments.Length != 3)
                        throw new InvalidOperationException($"Handler '{handlerType}' does not match the type arguments of the '{@event.Name}' event.");

                    var parameterType = eventTypeArguments[1];
                    var @interface = typeof(IShardedGatewayEventHandler<>).MakeGenericType(parameterType);

                    if (!handlerType.IsAssignableTo(@interface))
                        throw new InvalidOperationException($"Handler '{handlerType}' does not implement the required interface '{@interface}'.");

                    var method = @interface.GetMethod(nameof(IShardedGatewayEventHandler.HandleAsync))!;

                    @event.AddEventHandler(client, method.CreateDelegate(typeof(Func<,,>).MakeGenericType(typeof(GatewayClient), parameterType, typeof(ValueTask)), handler));
                }
            }
        }
    }
}
