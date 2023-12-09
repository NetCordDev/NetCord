using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
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
        return builder.ConfigureServices((context, services) =>
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
        });
    }
}
