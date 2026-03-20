using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.Gateway;

public static class GatewayHandlerServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IGatewayHandler
    {
        services.AddSingleton<IGatewayHandler, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IGatewayHandler
    {
        services.AddSingleton<IGatewayHandler, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler(this IServiceCollection services, GatewayEvent gatewayEvent, Delegate handler)
    {
        services.AddSingleton<IGatewayHandler>(services => new DelegateGatewayHandler(gatewayEvent.Name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler<T>(this IServiceCollection services, GatewayEvent<T> gatewayEvent, Delegate handler)
    {
        services.AddSingleton<IGatewayHandler>(services => new DelegateGatewayHandler<T>(gatewayEvent.Name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IGatewayHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IGatewayHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddGatewayHandlers(this IServiceCollection services, Assembly assembly)
    {
        AddGatewayHandlers(services, typeof(IGatewayHandler), assembly);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IShardedGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IShardedGatewayHandler
    {
        services.AddSingleton<IShardedGatewayHandler, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IShardedGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IShardedGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IShardedGatewayHandler
    {
        services.AddSingleton<IShardedGatewayHandler, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler(this IServiceCollection services, GatewayEvent gatewayEvent, Delegate handler)
    {
        services.AddSingleton<IShardedGatewayHandler>(services => new DelegateShardedGatewayHandler(gatewayEvent.Name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler<T>(this IServiceCollection services, GatewayEvent<T> gatewayEvent, Delegate handler)
    {
        services.AddSingleton<IShardedGatewayHandler>(services => new DelegateShardedGatewayHandler<T>(gatewayEvent.Name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IShardedGatewayHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IShardedGatewayHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddShardedGatewayHandlers(this IServiceCollection services, Assembly assembly)
    {
        AddGatewayHandlers(services, typeof(IShardedGatewayHandler), assembly);
        return services;
    }

    [RequiresUnreferencedCode("Types might be removed")]
    private static void AddGatewayHandlers(IServiceCollection services, Type handlerBase, Assembly assembly)
    {
        foreach (var type in HandlerHelpers.GetHandlers(handlerBase, assembly))
            services.AddSingleton(handlerBase, type);
    }
}
