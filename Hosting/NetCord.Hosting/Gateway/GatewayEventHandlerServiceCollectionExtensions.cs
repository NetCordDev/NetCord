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
    public static IServiceCollection AddGatewayEventHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IGatewayEventHandlerBase
    {
        services.AddSingleton<IGatewayEventHandlerBase, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IGatewayEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IGatewayEventHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IGatewayEventHandlerBase
    {
        services.AddSingleton<IGatewayEventHandlerBase, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> to.</param>
    /// <param name="name">The name of the event handler.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayEventHandler(this IServiceCollection services, string name, Delegate handler)
    {
        services.AddSingleton<IGatewayEventHandlerBase>(services => new DelegateGatewayEventHandler(name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> to.</param>
    /// <param name="name">The name of the event handler.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddGatewayEventHandler<T>(this IServiceCollection services, string name, Delegate handler)
    {
        services.AddSingleton<IGatewayEventHandlerBase>(services => new DelegateGatewayEventHandler<T>(name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds all <see cref="IGatewayEventHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IGatewayEventHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IGatewayEventHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
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

    /// <summary>
    /// Adds an <see cref="IShardedGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IShardedGatewayEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayEventHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IShardedGatewayEventHandlerBase
    {
        services.AddSingleton<IShardedGatewayEventHandlerBase, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IShardedGatewayEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IShardedGatewayEventHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IShardedGatewayEventHandlerBase
    {
        services.AddSingleton<IShardedGatewayEventHandlerBase, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> to.</param>
    /// <param name="name">The name of the event handler.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayEventHandler(this IServiceCollection services, string name, Delegate handler)
    {
        services.AddSingleton<IShardedGatewayEventHandlerBase>(services => new DelegateShardedGatewayEventHandler(name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IShardedGatewayEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> to.</param>
    /// <param name="name">The name of the event handler.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddShardedGatewayEventHandler<T>(this IServiceCollection services, string name, Delegate handler)
    {
        services.AddSingleton<IShardedGatewayEventHandlerBase>(services => new DelegateShardedGatewayEventHandler<T>(name, services, handler));
        return services;
    }

    /// <summary>
    /// Adds all <see cref="IShardedGatewayEventHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IShardedGatewayEventHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IShardedGatewayEventHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
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
