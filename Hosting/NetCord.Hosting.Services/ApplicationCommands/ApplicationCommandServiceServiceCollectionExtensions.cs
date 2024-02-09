using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public static class ApplicationCommandServiceServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationCommandService<TInteraction, TContext>(this IServiceCollection services)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return services.AddApplicationCommandService<TInteraction, TContext>((_, _) => { });
    }

    public static IServiceCollection AddApplicationCommandService<TInteraction, TContext>(this IServiceCollection services,
                                                                                                Action<ApplicationCommandServiceOptions<TInteraction, TContext>> configureOptions)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return services.AddApplicationCommandService<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddApplicationCommandService<TInteraction, TContext>(this IServiceCollection services,
                                                                                                Action<ApplicationCommandServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        services
            .AddOptions<ApplicationCommandServiceOptions<TInteraction, TContext>>()
            .Configure(configureOptions);

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<ApplicationCommandServiceOptions<TInteraction, TContext>>>().Value;
            return new ApplicationCommandService<TContext>(options.Configuration);
        });
        services.AddSingleton<IApplicationCommandService>(services => services.GetRequiredService<ApplicationCommandService<TContext>>());
        services.AddSingleton<IService>(services => services.GetRequiredService<ApplicationCommandService<TContext>>());

        services.AddSingleton<ApplicationCommandInteractionHandler<TInteraction, TContext>>();
        services.AddGatewayEventHandler(services => services.GetRequiredService<ApplicationCommandInteractionHandler<TInteraction, TContext>>());
        services.AddShardedGatewayEventHandler(services => services.GetRequiredService<ApplicationCommandInteractionHandler<TInteraction, TContext>>());
        services.AddHttpInteractionHandler(services => services.GetRequiredService<ApplicationCommandInteractionHandler<TInteraction, TContext>>());

        services.AddHostedService<ApplicationCommandServiceHostedService>();

        return services;
    }

    public static IServiceCollection AddApplicationCommandService<TInteraction, TContext, TAutocompleteContext>(this IServiceCollection services)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return services.AddApplicationCommandService<TInteraction, TContext, TAutocompleteContext>((_, _) => { });
    }

    public static IServiceCollection AddApplicationCommandService<TInteraction, TContext, TAutocompleteContext>(this IServiceCollection services,
                                                                                                                     Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>> configureOptions)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return services.AddApplicationCommandService<TInteraction, TContext, TAutocompleteContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddApplicationCommandService<TInteraction, TContext, TAutocompleteContext>(this IServiceCollection services,
                                                                                                                            Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>, IServiceProvider> configureOptions)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        services
            .AddOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>>()
            .Configure(configureOptions);

        services.AddSingleton<IOptions<ApplicationCommandServiceOptions<TInteraction, TContext>>>(services => services.GetRequiredService<IOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>>>());

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>>>().Value;
            return new ApplicationCommandService<TContext, TAutocompleteContext>(options.Configuration);
        });
        services.AddSingleton<ApplicationCommandService<TContext>>(services => services.GetRequiredService<ApplicationCommandService<TContext, TAutocompleteContext>>());
        services.AddSingleton<IApplicationCommandService>(services => services.GetRequiredService<ApplicationCommandService<TContext, TAutocompleteContext>>());
        services.AddSingleton<IService>(services => services.GetRequiredService<ApplicationCommandService<TContext, TAutocompleteContext>>());

        services.AddSingleton<ApplicationCommandInteractionHandler<TInteraction, TContext>>();
        services.AddGatewayEventHandler(services => services.GetRequiredService<ApplicationCommandInteractionHandler<TInteraction, TContext>>());
        services.AddShardedGatewayEventHandler(services => services.GetRequiredService<ApplicationCommandInteractionHandler<TInteraction, TContext>>());
        services.AddHttpInteractionHandler(services => services.GetRequiredService<ApplicationCommandInteractionHandler<TInteraction, TContext>>());

        services.AddSingleton<AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>>();
        services.AddGatewayEventHandler(services => services.GetRequiredService<AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>>());
        services.AddShardedGatewayEventHandler(services => services.GetRequiredService<AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>>());
        services.AddHttpInteractionHandler(services => services.GetRequiredService<AutocompleteInteractionHandler<TInteraction, TContext, TAutocompleteContext>>());

        services.AddHostedService<ApplicationCommandServiceHostedService>();

        return services;
    }
}
