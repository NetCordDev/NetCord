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
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IWebhookHandler
    {
        services.AddSingleton<IWebhookHandler, T>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IWebhookHandler"/> to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookHandler"/> to.</param>
    /// <param name="implementationFactory">The factory that creates the <see cref="IWebhookHandler"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IWebhookHandler
    {
        services.AddSingleton<IWebhookHandler, T>(implementationFactory);
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookHandler"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event handler argument.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookHandler"/> to.</param>
    /// <param name="webhookEvent">The webhook event.</param>
    /// <param name="handler">The delegate that represents the handler.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookHandler<T>(this IServiceCollection services, WebhookEvent<T> webhookEvent, Delegate handler)
    {
        services.AddSingleton<IWebhookHandler>(services => new DelegateWebhookHandler<T>(webhookEvent.RawName, services, handler));
        return services;
    }

    /// <summary>
    /// Adds all public <see cref="IWebhookHandler"/> implementations from the specified assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookHandler"/> implementations to.</param>
    /// <param name="assembly">The assembly to scan for <see cref="IWebhookHandler"/> implementations.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    [RequiresUnreferencedCode("Types might be removed")]
    public static IServiceCollection AddWebhookHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerBase = typeof(IWebhookHandler);

        foreach (var handler in HandlerHelpers.GetHandlers(handlerBase, assembly))
            services.AddSingleton(handlerBase, handler);

        return services;
    }
}
