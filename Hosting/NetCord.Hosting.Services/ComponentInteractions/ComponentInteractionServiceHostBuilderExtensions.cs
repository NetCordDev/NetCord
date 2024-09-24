using Microsoft.Extensions.Hosting;

using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public static class ComponentInteractionServiceHostBuilderExtensions
{
    // Configure

    public static IHostBuilder ConfigureComponentInteractions(
        this IHostBuilder builder,
        Action<ComponentInteractionServiceOptions> configureOptions)
    {
        return builder.ConfigureComponentInteractions((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureComponentInteractions(
        this IHostBuilder builder,
        Action<ComponentInteractionServiceOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.ConfigureComponentInteractions(configureOptions));
    }

    public static IHostBuilder ConfigureComponentInteractions<TInteraction,
                                                              TContext>(
        this IHostBuilder builder,
        Action<ComponentInteractionServiceOptions<TInteraction, TContext>> configureOptions)

        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.ConfigureComponentInteractions<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureComponentInteractions<TInteraction,
                                                              TContext>(
        this IHostBuilder builder,
        Action<ComponentInteractionServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)

        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.ConfigureServices((context, services) => services.ConfigureComponentInteractions(configureOptions));
    }

    // Use

    public static IHostBuilder UseComponentInteractions<TInteraction,
                                                        [DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder)

        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.UseComponentInteractions<TInteraction, TContext>((_, _) => { });
    }

    public static IHostBuilder UseComponentInteractions<TInteraction,
                                                        [DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder,
        Action<ComponentInteractionServiceOptions<TInteraction, TContext>> configureOptions)

        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.UseComponentInteractions<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseComponentInteractions<TInteraction,
                                                        [DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder,
        Action<ComponentInteractionServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)

        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return builder.ConfigureServices((context, services) => services.AddComponentInteractions(configureOptions));
    }
}
