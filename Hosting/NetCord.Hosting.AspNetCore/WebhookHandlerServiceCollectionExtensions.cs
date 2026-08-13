using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.AspNetCore;

public static class WebhookHandlerServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IWebhookHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IWebhookHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookHandler"/> to.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IWebhookHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IWebhookHandler
    {
        AddWebhookHandlerCore(services, typeof(T), lifetime);

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IWebhookHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IWebhookHandler"/>.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IWebhookHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory, ServiceLifetime lifetime = ServiceLifetime.Singleton) where T : class, IWebhookHandler
    {
        services.AddSingleton<IWebhookHandlerMetadata>(ClassHandlerMetadata<IWebhookHandler>.CreateWithFactory(typeof(T), lifetime is ServiceLifetime.Singleton, implementationFactory));

        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookHandler"/> to.</param>
    /// <param name="handlerType">The type of the <see cref="IWebhookHandler"/> to add.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IWebhookHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookHandler(this IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        HandlerHelpers.EnsureHandlerTypeIsValid(handlerType, typeof(IWebhookHandler));

        AddWebhookHandlerCore(services, handlerType, lifetime);

        return services;
    }

    private static void AddWebhookHandlerCore(IServiceCollection services, [DAM(DAMT.PublicConstructors)] Type handlerType, ServiceLifetime lifetime)
    {
        services.AddSingleton<IWebhookHandlerMetadata>(ClassHandlerMetadata<IWebhookHandler>.Create(handlerType, lifetime is ServiceLifetime.Singleton));
    }

    /// <summary>
    /// Adds an <see cref="IWebhookHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookHandler"/> to.</param>
    /// <param name="webhookEvent">The webhook event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IWebhookHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookHandler<T>(this IServiceCollection services, WebhookEvent<T> webhookEvent, Delegate handler, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        services.AddSingleton<IWebhookHandlerMetadata>(new DelegateHandlerMetadata<WebhookEventId>(handler, webhookEvent.EventId, lifetime is ServiceLifetime.Singleton));

        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IWebhookHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IWebhookHandler"/> implementations.</param>
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the <see cref="IWebhookHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddWebhookHandlers(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        var handlerBase = typeof(IWebhookHandler);

        foreach (var handler in HandlerHelpers.GetHandlers(handlerBase, assembly))
            AddWebhookHandlerCore(services, handler, lifetime);

        return services;
    }
}
