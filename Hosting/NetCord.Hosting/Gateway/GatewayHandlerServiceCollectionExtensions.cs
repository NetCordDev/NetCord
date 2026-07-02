using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NetCord.Hosting.Gateway;

internal abstract class GatewayHandlerMetadata(bool isSingleton)
{
    public bool IsSingleton => isSingleton;
}

internal sealed class ClassGatewayHandlerMetadata(Type handlerType, bool isSingleton) : GatewayHandlerMetadata(isSingleton)
{
    public Type HandlerType => handlerType;

}

internal sealed class DelegateGatewayHandlerMetadata(Delegate handler, GatewayEventId eventId, bool isSingleton) : GatewayHandlerMetadata(isSingleton)
{
    public Delegate Handler => handler;

    public GatewayEventId EventId => eventId;
}

public static class GatewayHandlerServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IGatewayHandler
    {
        services.TryAdd(ServiceDescriptor.Describe(typeof(T), typeof(T), lifetime));
        services.AddSingleton<GatewayHandlerMetadata>(new ClassGatewayHandlerMetadata(typeof(T), lifetime is ServiceLifetime.Singleton));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IGatewayHandler
    {
        services.TryAdd(ServiceDescriptor.Describe(typeof(T), implementationFactory, lifetime));
        services.AddSingleton<GatewayHandlerMetadata>(new ClassGatewayHandlerMetadata(typeof(T), lifetime is ServiceLifetime.Singleton));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="handlerType">The type of the <see cref="IGatewayHandler"/> to add.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler(this IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.TryAdd(ServiceDescriptor.Describe(handlerType, handlerType, lifetime));
        services.AddSingleton<GatewayHandlerMetadata>(new ClassGatewayHandlerMetadata(handlerType, lifetime is ServiceLifetime.Singleton));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler(this IServiceCollection services, GatewayEvent gatewayEvent, Delegate handler, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<GatewayHandlerMetadata>(new DelegateGatewayHandlerMetadata(
            DelegateHandlerHelper.CreateHandler<Func<IServiceProvider, ValueTask>>(handler, []),
            gatewayEvent.Id,
            lifetime is ServiceLifetime.Singleton));

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
    public static IServiceCollection AddGatewayHandler<T>(this IServiceCollection services, GatewayEvent<T> gatewayEvent, Delegate handler, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<GatewayHandlerMetadata>(new DelegateGatewayHandlerMetadata(
            DelegateHandlerHelper.CreateHandler<Func<T, IServiceProvider, ValueTask>>(handler, [typeof(T)]),
            gatewayEvent.Id,
            lifetime is ServiceLifetime.Singleton));

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
    /// <param name="handlerType">The type of the <see cref="IShardedGatewayHandler"/> to add.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler(this IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType)
    {
        services.AddSingleton(typeof(IShardedGatewayHandler), handlerType);
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
        services.AddSingleton<IShardedGatewayHandler>(services => new DelegateShardedGatewayHandler("", services, handler));
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
        services.AddSingleton<IShardedGatewayHandler>(services => new DelegateShardedGatewayHandler<T>("", services, handler));
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
