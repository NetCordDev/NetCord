using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.Gateway;

public static class GatewayEventHandlerServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayEventHandler<T>(this IServiceCollection services) where T : class, IGatewayEventHandlerBase
    {
        services.AddSingleton<IGatewayEventHandlerBase, T>();
        return services;
    }

    public static IServiceCollection AddGatewayEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IGatewayEventHandlerBase
    {
        services.AddSingleton<IGatewayEventHandlerBase, T>(implementationFactory);
        return services;
    }

    public static IServiceCollection AddGatewayEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerBase = typeof(IGatewayEventHandlerBase);

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            if (type.IsAssignableTo(handlerBase))
                services.AddSingleton(handlerBase, type);
        }

        return services;
    }

    public static IServiceCollection AddShardedGatewayEventHandler<T>(this IServiceCollection services) where T : class, IShardedGatewayEventHandlerBase
    {
        services.AddSingleton<IShardedGatewayEventHandlerBase, T>();
        return services;
    }

    public static IServiceCollection AddShardedGatewayEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IShardedGatewayEventHandlerBase
    {
        services.AddSingleton<IShardedGatewayEventHandlerBase, T>(implementationFactory);
        return services;
    }

    public static IServiceCollection AddShardedGatewayEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerBase = typeof(IShardedGatewayEventHandlerBase);

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            if (type.IsAssignableTo(handlerBase))
                services.AddSingleton(handlerBase, type);
        }

        return services;
    }
}
