using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.Gateway;

public static class GatewayEventHandlerServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayEventHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IGatewayEventHandler
    {
        services.AddSingleton<IGatewayEventHandler, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IGatewayEventHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IGatewayEventHandler
    {
        services.AddSingleton<IGatewayEventHandler, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayEventHandler(this IServiceCollection services, GatewayEvent gatewayEvent, Delegate handler)
    {
        services.AddSingleton<IGatewayEventHandler>(services => new DelegateGatewayEventHandler(gatewayEvent.Name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayEventHandler<T>(this IServiceCollection services, GatewayEvent<T> gatewayEvent, Delegate handler)
    {
        services.AddSingleton<IGatewayEventHandler>(services => new DelegateGatewayEventHandler<T>(gatewayEvent.Name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IGatewayEventHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IGatewayEventHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddGatewayEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        AddGatewayEventHandlers(services, typeof(IGatewayEventHandler), assembly);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IShardedGatewayEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayEventHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IShardedGatewayEventHandler
    {
        services.AddSingleton<IShardedGatewayEventHandler, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IShardedGatewayEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IShardedGatewayEventHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IShardedGatewayEventHandler
    {
        services.AddSingleton<IShardedGatewayEventHandler, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayEventHandler(this IServiceCollection services, GatewayEvent gatewayEvent, Delegate handler)
    {
        services.AddSingleton<IShardedGatewayEventHandler>(services => new DelegateShardedGatewayEventHandler(gatewayEvent.Name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayEventHandler<T>(this IServiceCollection services, GatewayEvent<T> gatewayEvent, Delegate handler)
    {
        services.AddSingleton<IShardedGatewayEventHandler>(services => new DelegateShardedGatewayEventHandler<T>(gatewayEvent.Name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IShardedGatewayEventHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IShardedGatewayEventHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddShardedGatewayEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        AddGatewayEventHandlers(services, typeof(IShardedGatewayEventHandler), assembly);
        return services;
    }

    [RequiresUnreferencedCode("Types might be removed")]
    private static void AddGatewayEventHandlers(IServiceCollection services, Type handlerBase, Assembly assembly)
    {
        foreach (var type in HandlerHelpers.GetHandlers(handlerBase, assembly))
            services.AddSingleton(handlerBase, type);
    }
}
