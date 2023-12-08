using System.Reflection;

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
            var handlerType = handler.GetType();

            var attributes = handlerType.GetCustomAttributes<GatewayEventAttribute>();

            foreach (var attribute in attributes)
            {
                if (events.TryGetValue(attribute.Name, out var @event))
                {
                    if (handler is IGatewayEventHandler gatewayEventHandler)
                    {
                        if (@event.EventHandlerType!.GenericTypeArguments.Length != 1)
                            throw new InvalidOperationException($"Handler '{handlerType}' does not match the type arguments of the '{@event.Name}' event.");

                        @event.AddEventHandler(client, gatewayEventHandler.HandleAsync);
                    }
                    else
                    {
                        foreach (var @interface in handlerType.GetInterfaces())
                        {
                            if (!@interface.IsGenericType || @interface.GetGenericTypeDefinition() != typeof(IGatewayEventHandler<>))
                                continue;

                            var parameterType = @interface.GenericTypeArguments[0];

                            var eventTypeArguments = @event.EventHandlerType!.GenericTypeArguments;

                            if (eventTypeArguments.Length != 2 || eventTypeArguments[0] != parameterType)
                                throw new InvalidOperationException($"Handler '{handlerType}' does not match the type arguments of the '{@event.Name}' event.");

                            var method = @interface.GetMethod(nameof(IGatewayEventHandler.HandleAsync))!;

                            @event.AddEventHandler(client, method.CreateDelegate(typeof(Func<,>).MakeGenericType(parameterType, typeof(ValueTask)), handler));
                        }
                    }
                }
                else
                    throw new InvalidOperationException($"Event '{attribute.Name}' does not exist for '{nameof(GatewayClient)}'.");
            }
        }
    }

    private static void RegisterShardedGatewayClientHandlers(ShardedGatewayClient client, IServiceProvider services)
    {
        var events = typeof(ShardedGatewayClient).GetEvents().ToDictionary(e => e.Name);

        foreach (var handler in services.GetServices<IShardedGatewayEventHandlerBase>())
        {
            var handlerType = handler.GetType();

            var attributes = handlerType.GetCustomAttributes<GatewayEventAttribute>();

            foreach (var attribute in attributes)
            {
                if (events.TryGetValue(attribute.Name, out var @event))
                {
                    if (handler is IShardedGatewayEventHandler shardedGatewayEventHandler)
                    {
                        if (@event.EventHandlerType!.GenericTypeArguments.Length != 1)
                            throw new InvalidOperationException($"Handler '{handlerType}' does not match the type arguments of the '{@event.Name}' event.");

                        @event.AddEventHandler(client, shardedGatewayEventHandler.HandleAsync);
                    }
                    else
                    {
                        foreach (var @interface in handlerType.GetInterfaces())
                        {
                            if (!@interface.IsGenericType || @interface.GetGenericTypeDefinition() != typeof(IShardedGatewayEventHandler<>))
                                continue;

                            var parameterType = @interface.GenericTypeArguments[0];

                            var eventTypeArguments = @event.EventHandlerType!.GenericTypeArguments;

                            if (eventTypeArguments.Length != 3 || eventTypeArguments[1] != parameterType)
                                throw new InvalidOperationException($"Handler '{handlerType}' does not match the type arguments of the '{@event.Name}' event.");

                            var method = @interface.GetMethod(nameof(IShardedGatewayEventHandler.HandleAsync))!;

                            @event.AddEventHandler(client, method.CreateDelegate(typeof(Func<,,>).MakeGenericType(typeof(GatewayClient), parameterType, typeof(ValueTask)), handler));
                        }
                    }
                }
                else
                    throw new InvalidOperationException($"Event '{attribute.Name}' does not exist for '{nameof(ShardedGatewayClient)}'.");

            }
        }
    }
}
