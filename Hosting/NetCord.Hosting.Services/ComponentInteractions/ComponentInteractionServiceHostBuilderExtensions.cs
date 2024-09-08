using Microsoft.Extensions.Hosting;

using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public static class ComponentInteractionServiceHostBuilderExtensions
{
    public static IHostBuilder UseComponentInteractions<TInteraction, TContext>(this IHostBuilder builder)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.UseComponentInteractions<TInteraction, TContext>((_, _) => { });
    }

    public static IHostBuilder UseComponentInteractions<TInteraction, TContext>(this IHostBuilder builder,
                                                                                Action<ComponentInteractionServiceOptions<TInteraction, TContext>> configureOptions)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.UseComponentInteractions<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseComponentInteractions<TInteraction, TContext>(this IHostBuilder builder,
                                                                                Action<ComponentInteractionServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.ConfigureServices((context, services) => services.AddComponentInteractions(configureOptions));
    }
}
