using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting;

public static class HttpInteractionHandlerServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IHttpInteractionHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IHttpInteractionHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionHandler"/> to.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IHttpInteractionHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IHttpInteractionHandler
    {
        return AddHttpInteractionHandler(services, typeof(T), lifetime);
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IHttpInteractionHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IHttpInteractionHandler"/>.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IHttpInteractionHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IHttpInteractionHandler
    {
        services.AddSingleton<HttpInteractionHandlerMetadata>(new ClassHttpInteractionHandlerMetadata(typeof(T), lifetime is ServiceLifetime.Singleton, implementationFactory));

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionHandler"/> to.</param>
    /// <param name="handlerType">The type of the <see cref="IHttpInteractionHandler"/> to add.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IHttpInteractionHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionHandler(this IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var isSingleton = lifetime is ServiceLifetime.Singleton;

        services.AddSingleton<HttpInteractionHandlerMetadata>(new ClassHttpInteractionHandlerMetadata(handlerType, isSingleton, HandlerHelpers.CreateInstanceFactory(handlerType, isSingleton)));

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionHandler"/> to.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IHttpInteractionHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionHandler(this IServiceCollection services, Delegate handler, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<HttpInteractionHandlerMetadata>(new DelegateHttpInteractionHandlerMetadata(
            DelegateHandlerHelper.CreateHandler<Func<Interaction, IServiceProvider, ValueTask>>(handler, [typeof(Interaction)]),
            lifetime is ServiceLifetime.Singleton));

        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IHttpInteractionHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IHttpInteractionHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddHttpInteractionHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerBase = typeof(IHttpInteractionHandler);

        foreach (var handler in HandlerHelpers.GetHandlers(handlerBase, assembly))
            AddHttpInteractionHandler(services, handler);

        return services;
    }
}
