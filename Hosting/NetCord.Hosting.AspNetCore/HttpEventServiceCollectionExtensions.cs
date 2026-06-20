using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.AspNetCore;

public static class HttpEventServiceCollectionExtensions
{
    /// <summary>
    /// Adds an <see cref="IHttpInteractionProcessor"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IHttpInteractionProcessor"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddHttpInteractionProcessor(this IServiceCollection services)
    {
        return services.AddSingleton<IHttpInteractionProcessor, HttpInteractionProcessor>();
    }

    /// <summary>
    /// Adds an <see cref="IWebhookEventProcessor"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="IWebhookEventProcessor"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddWebhookEventProcessor(this IServiceCollection services)
    {
        return services.AddSingleton<IWebhookEventProcessor, WebhookEventProcessor>();
    }
}

