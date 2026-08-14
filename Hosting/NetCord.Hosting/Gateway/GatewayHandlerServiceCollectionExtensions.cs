using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static class GatewayHandlerServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IGatewayHandler
    {
        AddGatewayHandlerCore(services, typeof(T), lifetime);

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IGatewayHandler"/>.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IGatewayHandler
    {
        var handlerMetadata = ClassHandlerMetadata.CreateWithFactory(
            typeof(T),
            lifetime is ServiceLifetime.Singleton,
            implementationFactory);

        services.AddSingleton<IGatewayHandlerMetadata>(_ => handlerMetadata);

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="handlerType">The type of the <see cref="IGatewayHandler"/> to add.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler(this IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        HandlerHelpers.EnsureHandlerTypeIsValid(handlerType, typeof(IGatewayHandler));

        AddGatewayHandlerCore(services, handlerType, lifetime);

        return services;
    }

    private static void AddGatewayHandlerCore(IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType, ServiceLifetime lifetime)
    {
        var handlerMetadata = ClassHandlerMetadata.Create(
            handlerType,
            lifetime is ServiceLifetime.Singleton);

        services.AddSingleton<IGatewayHandlerMetadata>(_ => handlerMetadata);
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler(this IServiceCollection services, GatewayEvent gatewayEvent, Delegate handler, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var handlerMetadata = DelegateHandlerMetadata<GatewayEventId>.Create<Func<IServiceProvider, ValueTask>>(
            handler,
            gatewayEvent.Id,
            lifetime is ServiceLifetime.Singleton,
            []);

        services.AddSingleton<IGatewayHandlerMetadata>(handlerMetadata);

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayHandler<T>(this IServiceCollection services, GatewayEvent<T> gatewayEvent, Delegate handler, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var handlerMetadata = DelegateHandlerMetadata<GatewayEventId>.Create<Func<T, IServiceCollection, ValueTask>>(
            handler,
            gatewayEvent.Id,
            lifetime is ServiceLifetime.Singleton,
            [typeof(T)]);

        services.AddSingleton<IGatewayHandlerMetadata>(handlerMetadata);

        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IGatewayHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IGatewayHandler"/> implementations.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IGatewayHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddGatewayHandlers(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        foreach (var type in HandlerHelpers.GetHandlers(typeof(IGatewayHandler), assembly))
            AddGatewayHandlerCore(services, type, lifetime);

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IShardedGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IShardedGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IShardedGatewayHandler
    {
        AddShardedGatewayHandlerCore(services, typeof(T), lifetime);

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IShardedGatewayHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IShardedGatewayHandler"/>.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IShardedGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IShardedGatewayHandler
    {
        var handlerMetadata = ClassHandlerMetadata.CreateWithFactory(
            typeof(T),
            lifetime is ServiceLifetime.Singleton,
            implementationFactory);

        services.AddSingleton<IShardedGatewayHandlerMetadata>(_ => handlerMetadata);

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <param name="handlerType">The type of the <see cref="IShardedGatewayHandler"/> to add.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IShardedGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler(this IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        HandlerHelpers.EnsureHandlerTypeIsValid(handlerType, typeof(IShardedGatewayHandler));

        AddShardedGatewayHandlerCore(services, handlerType, lifetime);

        return services;
    }

    private static void AddShardedGatewayHandlerCore(IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType, ServiceLifetime lifetime)
    {
        var handlerMetadata = ClassHandlerMetadata.Create(
            handlerType,
            lifetime is ServiceLifetime.Singleton);

        services.AddSingleton<IShardedGatewayHandlerMetadata>(_ => handlerMetadata);
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IShardedGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler(this IServiceCollection services, GatewayEvent gatewayEvent, Delegate handler, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var handlerMetadata = DelegateHandlerMetadata<GatewayEventId>.Create<Func<GatewayClient, IServiceProvider, ValueTask>>(
            handler,
            gatewayEvent.Id,
            lifetime is ServiceLifetime.Singleton,
            [typeof(GatewayClient)]);

        services.AddSingleton<IShardedGatewayHandlerMetadata>(handlerMetadata);

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> to.</param>
    /// <param name="gatewayEvent">The gateway event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IShardedGatewayHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayHandler<T>(this IServiceCollection services, GatewayEvent<T> gatewayEvent, Delegate handler, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var handlerMetadata = DelegateHandlerMetadata<GatewayEventId>.Create<Func<GatewayClient, T, IServiceProvider, ValueTask>>(
            handler,
            gatewayEvent.Id,
            lifetime is ServiceLifetime.Singleton,
            [typeof(GatewayClient), typeof(T)]);

        services.AddSingleton<IShardedGatewayHandlerMetadata>(handlerMetadata);

        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IShardedGatewayHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IShardedGatewayHandler"/> implementations.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IShardedGatewayHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddShardedGatewayHandlers(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        foreach (var type in HandlerHelpers.GetHandlers(typeof(IShardedGatewayHandler), assembly))
            AddShardedGatewayHandlerCore(services, type, lifetime);

        return services;
    }
}
