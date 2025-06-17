using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.AspNetCore;

public static class WebhookEventHandlerServiceCollectionExtensions
{

    /// <summary>
    /// Adds an <see cref="IWebhookEventHandler{T}"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IWebhookEventHandler{T}"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandler{T}"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IWebhookEventHandlerBase
    {
        services.AddSingleton<IWebhookEventHandlerBase, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventHandler{T}"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IWebhookEventHandler{T}"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandler{T}"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IWebhookEventHandler{T}"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IWebhookEventHandlerBase
    {
        services.AddSingleton<IWebhookEventHandlerBase, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventHandler{T}"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandler{T}"/> to.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventHandler<T>(this IServiceCollection services, Delegate handler)
    {
        services.AddSingleton<IWebhookEventHandlerBase>(services => new DelegateWebhookEventHandler<T>(services, handler));
        return services;
    }

    /// <summary>
    /// Adds all <see cref="IWebhookEventHandler{T}"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandler{T}"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IWebhookEventHandler{T}"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddWebhookEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerBase = typeof(IWebhookEventHandlerBase);

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
