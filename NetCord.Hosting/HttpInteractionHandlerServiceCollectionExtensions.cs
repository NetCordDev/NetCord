using Microsoft.Extensions.DependencyInjection;

namespace NetCord.Hosting;

public static class HttpInteractionHandlerServiceCollectionExtensions
{
    public static IServiceCollection AddHttpInteractionHandler<T>(this IServiceCollection services) where T : class, IHttpInteractionHandler
    {
        services.AddSingleton<IHttpInteractionHandler, T>();
        return services;
    }

    public static IServiceCollection AddHttpInteractionHandler<T>(this IServiceCollection services, Func<IServiceProvider, T> implementationFactory) where T : class, IHttpInteractionHandler
    {
        services.AddSingleton<IHttpInteractionHandler, T>(implementationFactory);
        return services;
    }
}
