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
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IHttpInteractionHandler
    {
        services.AddSingleton<IHttpInteractionHandler, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IHttpInteractionHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IHttpInteractionHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IHttpInteractionHandler
    {
        services.AddSingleton<IHttpInteractionHandler, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionHandler"/> to.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionHandler(this IServiceCollection services, Delegate handler)
    {
        services.AddSingleton<IHttpInteractionHandler>(services => new DelegateHttpInteractionHandler(services, handler));
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
            services.AddSingleton(handlerBase, handler);

        return services;
    }
}
