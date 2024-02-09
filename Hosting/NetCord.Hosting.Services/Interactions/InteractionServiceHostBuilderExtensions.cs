using Microsoft.Extensions.Hosting;

using NetCord.Services.Interactions;

namespace NetCord.Hosting.Services.Interactions;

public static class InteractionServiceHostBuilderExtensions
{
    public static IHostBuilder UseInteractionService<TInteraction, TContext>(this IHostBuilder builder)
        where TInteraction : Interaction
        where TContext : IInteractionContext
    {
        return builder.UseInteractionService<TInteraction, TContext>((_, _) => { });
    }

    public static IHostBuilder UseInteractionService<TInteraction, TContext>(this IHostBuilder builder,
                                                                             Action<InteractionServiceOptions<TInteraction, TContext>> configureOptions)
        where TInteraction : Interaction
        where TContext : IInteractionContext
    {
        return builder.UseInteractionService<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseInteractionService<TInteraction, TContext>(this IHostBuilder builder,
                                                                             Action<InteractionServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)
        where TInteraction : Interaction
        where TContext : IInteractionContext
    {
        return builder.ConfigureServices((context, services) => services.AddInteractionService(configureOptions));
    }
}
