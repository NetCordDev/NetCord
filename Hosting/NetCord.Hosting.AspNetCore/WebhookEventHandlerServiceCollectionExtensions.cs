using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting.AspNetCore;

public static class WebhookEventHandlerServiceCollectionExtensions
{
    public static IServiceCollection AddWebhookEventHandler<[DAM(DAMT.PublicConstructors)] T>(this IServiceCollection services) where T : class, IWebhookEventHandlerBase
    {
        services.AddSingleton<IWebhookEventHandlerBase, T>();
        return services;
    }

    public static IServiceCollection AddWebhookEventHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IWebhookEventHandlerBase
    {
        services.AddSingleton<IWebhookEventHandlerBase, T>(implementationFactory);
        return services;
    }

    public static IServiceCollection AddWebhookEventHandler<T>(this IServiceCollection services, Delegate handler)
    {
        services.AddSingleton<IWebhookEventHandlerBase>(services => new DelegateWebhookEventHandler<T>(services, handler));
        return services;
    }
}
