using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.Interactions;

namespace NetCord.Hosting.Services.Interactions;

public static class InteractionServiceServiceCollectionExtensions
{
    public static IServiceCollection AddInteractionService<TInteraction, TContext>(this IServiceCollection services)
        where TInteraction : Interaction
        where TContext : IInteractionContext
    {
        return services.AddInteractionService<TInteraction, TContext>((_, _) => { });
    }

    public static IServiceCollection AddInteractionService<TInteraction, TContext>(this IServiceCollection services,
                                                                                        Action<InteractionServiceOptions<TInteraction, TContext>> configureOptions)
        where TInteraction : Interaction
        where TContext : IInteractionContext
    {
        return services.AddInteractionService<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddInteractionService<TInteraction, TContext>(this IServiceCollection services,
                                                                                        Action<InteractionServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)
        where TInteraction : Interaction
        where TContext : IInteractionContext
    {
        services
            .AddOptions<InteractionServiceOptions<TInteraction, TContext>>()
            .Configure(configureOptions);

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<InteractionServiceOptions<TInteraction, TContext>>>().Value;
            return new InteractionService<TContext>(options.Configuration);
        });
        services.AddSingleton<IService>(services => services.GetRequiredService<InteractionService<TContext>>());

        services.AddSingleton<InteractionHandler<TInteraction, TContext>>();
        services.AddGatewayEventHandler(services => services.GetRequiredService<InteractionHandler<TInteraction, TContext>>());
        services.AddShardedGatewayEventHandler(services => services.GetRequiredService<InteractionHandler<TInteraction, TContext>>());
        services.AddHttpInteractionHandler(services => services.GetRequiredService<InteractionHandler<TInteraction, TContext>>());

        return services;
    }
}
