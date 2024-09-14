﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Hosting.Services.ComponentInteractions;

public static class ComponentInteractionServiceServiceCollectionExtensions
{
    // Configure

    public static IServiceCollection ConfigureComponentInteractions(this IServiceCollection services,
                                                                    Action<ComponentInteractionServiceOptions> configureOptions)
    {
        return services.ConfigureComponentInteractions((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureComponentInteractions(this IServiceCollection services,
                                                                    Action<ComponentInteractionServiceOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<ComponentInteractionServiceOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    public static IServiceCollection ConfigureComponentInteractions<TInteraction, TContext>(this IServiceCollection services,
                                                                                            Action<ComponentInteractionServiceOptions<TInteraction, TContext>> configureOptions)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return services.ConfigureComponentInteractions<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureComponentInteractions<TInteraction, TContext>(this IServiceCollection services,
                                                                                            Action<ComponentInteractionServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        services
            .AddOptions<ComponentInteractionServiceOptions<TInteraction, TContext>>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    public static IServiceCollection AddComponentInteractions<TInteraction, TContext>(this IServiceCollection services)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return services.AddComponentInteractions<TInteraction, TContext>((_, _) => { });
    }

    public static IServiceCollection AddComponentInteractions<TInteraction, TContext>(this IServiceCollection services,
                                                                                      Action<ComponentInteractionServiceOptions<TInteraction, TContext>> configureOptions)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        return services.AddComponentInteractions<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddComponentInteractions<TInteraction, TContext>(this IServiceCollection services,
                                                                                      Action<ComponentInteractionServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)
        where TInteraction : ComponentInteraction
        where TContext : IComponentInteractionContext
    {
        services
            .AddOptions<ComponentInteractionServiceOptions<TInteraction, TContext>>()
            .BindConfiguration("Discord")
            .BindConfiguration("Discord:ComponentInteractions")
            .Configure<IOptions<ComponentInteractionServiceOptions>>((options, baseOptions) => options.Apply(baseOptions))
            .PostConfigure(configureOptions);

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<ComponentInteractionServiceOptions<TInteraction, TContext>>>().Value;
            return new ComponentInteractionService<TContext>(options.CreateConfiguration());
        });
        services.AddSingleton<IService>(services => services.GetRequiredService<ComponentInteractionService<TContext>>());

        services.AddSingleton<ComponentInteractionHandler<TInteraction, TContext>>();
        services.AddGatewayEventHandler(services => services.GetRequiredService<ComponentInteractionHandler<TInteraction, TContext>>());
        services.AddShardedGatewayEventHandler(services => services.GetRequiredService<ComponentInteractionHandler<TInteraction, TContext>>());
        services.AddHttpInteractionHandler(services => services.GetRequiredService<ComponentInteractionHandler<TInteraction, TContext>>());

        return services;
    }
}
