using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.AspNetCore;

public static class WebhookEventHandlerServiceCollectionExtensions
{

    /// <summary>
    /// Adds an <see cref="IWebhookEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IWebhookEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandler"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IWebhookEventHandler
    {
        services.AddSingleton<IWebhookEventHandler, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IWebhookEventHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IWebhookEventHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IWebhookEventHandler
    {
        services.AddSingleton<IWebhookEventHandler, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandler"/> to.</param>
    /// <param name="webhookEvent">The webhook event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventHandler<T>(this IServiceCollection services, WebhookEvent<T> webhookEvent, Delegate handler)
    {
        services.AddSingleton<IWebhookEventHandler>(services => new DelegateWebhookEventHandler<T>(webhookEvent.RawName, services, handler));
        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IWebhookEventHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IWebhookEventHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddWebhookEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerBase = typeof(IWebhookEventHandler);

        foreach (var handler in HandlerHelpers.GetHandlers(handlerBase, assembly))
            services.AddSingleton(handlerBase, handler);

        return services;
    }
}
