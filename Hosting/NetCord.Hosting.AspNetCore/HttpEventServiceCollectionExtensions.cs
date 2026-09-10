using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.AspNetCore;

public static class HttpEventServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IHttpInteractionParser"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionParser"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionParser(this IServiceCollection services)
    {
        services.AddSingleton<IHttpInteractionParser, HttpInteractionParser>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionHandlerInvoker"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionHandlerInvoker"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionHandlerInvoker(this IServiceCollection services)
    {
        services.AddSingleton<IHttpInteractionHandlerInvoker, HttpInteractionHandlerInvoker>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IHttpInteractionProcessor"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionProcessor"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionProcessor(this IServiceCollection services)
    {
        services.AddSingleton<IHttpInteractionProcessor, HttpInteractionProcessor>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventParser"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventParser"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventParser(this IServiceCollection services)
    {
        services.AddSingleton<IWebhookEventParser, WebhookEventParser>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventHandlerInvoker"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventHandlerInvoker"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventHandlerInvoker(this IServiceCollection services)
    {
        services.AddSingleton<IWebhookEventHandlerInvoker, WebhookEventHandlerInvoker>();
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventProcessor"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventProcessor"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventProcessor(this IServiceCollection services)
    {
        services.AddSingleton<IWebhookEventProcessor, WebhookEventProcessor>();
        return services;
    }
}
