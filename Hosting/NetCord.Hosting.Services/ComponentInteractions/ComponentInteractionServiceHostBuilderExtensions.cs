using Microsoft.Extensions.Hosting;

using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public static class ComponentInteractionServiceHostBuilderExtensions
{
    public static IHostBuilder UseComponentInteractionService<TInteraction, TContext>(this IHostBuilder builder)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.UseComponentInteractionService<TInteraction, TContext>((_, _) => { });
    }

    public static IHostBuilder UseComponentInteractionService<TInteraction, TContext>(this IHostBuilder builder,
                                                                                      Action<ComponentInteractionServiceOptions<TInteraction, TContext>> configureOptions)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.UseComponentInteractionService<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseComponentInteractionService<TInteraction, TContext>(this IHostBuilder builder,
                                                                                      Action<ComponentInteractionServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.ConfigureServices((context, services) => services.AddComponentInteractionService(configureOptions));
    }
}
