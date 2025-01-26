using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public static class ApplicationCommandServiceServiceCollectionExtensions
{
    // Configure

    public static IServiceCollection ConfigureApplicationCommands(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions> configureOptions)
    {
        return services.ConfigureApplicationCommands((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureApplicationCommands(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<ApplicationCommandServiceOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    public static IServiceCollection ConfigureApplicationCommands<TInteraction,
                                                                  TContext>(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext>> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return services.ConfigureApplicationCommands<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureApplicationCommands<TInteraction,
                                                                  TContext>(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        services
            .AddOptions<ApplicationCommandServiceOptions<TInteraction, TContext>>()
            .PostConfigure(configureOptions);

        return services;
    }

    public static IServiceCollection ConfigureApplicationCommands<TInteraction,
                                                                  TContext,
                                                                  TAutocompleteContext>(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return services.ConfigureApplicationCommands<TInteraction, TContext, TAutocompleteContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureApplicationCommands<TInteraction,
                                                                  TContext,
                                                                  TAutocompleteContext>(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>, IServiceProvider> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        services
            .AddOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    public static IServiceCollection AddApplicationCommands(
        this IServiceCollection services)
    {
        return services.AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>((_, _) => { });
    }

    public static IServiceCollection AddApplicationCommands(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>> configureOptions)
    {
        return services.AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddApplicationCommands(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>, IServiceProvider> configureOptions)
    {
        return services.AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>(configureOptions);
    }

    public static IServiceCollection AddHttpApplicationCommands(
        this IServiceCollection services)
    {
        return services.AddApplicationCommands<ApplicationCommandInteraction, HttpApplicationCommandContext, HttpAutocompleteInteractionContext>((_, _) => { });
    }

    public static IServiceCollection AddHttpApplicationCommands(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<ApplicationCommandInteraction, HttpApplicationCommandContext, HttpAutocompleteInteractionContext>> configureOptions)
    {
        return services.AddApplicationCommands<ApplicationCommandInteraction, HttpApplicationCommandContext, HttpAutocompleteInteractionContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddHttpApplicationCommands(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<ApplicationCommandInteraction, HttpApplicationCommandContext, HttpAutocompleteInteractionContext>, IServiceProvider> configureOptions)
    {
        return services.AddApplicationCommands(configureOptions);
    }

    public static IServiceCollection AddApplicationCommands<TInteraction,
                                                            [DAM(DAMT.PublicConstructors)] TContext>(
        this IServiceCollection services)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return services.AddApplicationCommands<TInteraction, TContext>((_, _) => { });
    }

    public static IServiceCollection AddApplicationCommands<TInteraction,
                                                            [DAM(DAMT.PublicConstructors)] TContext>(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext>> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return services.AddApplicationCommands<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddApplicationCommands<TInteraction,
                                                            [DAM(DAMT.PublicConstructors)] TContext>(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        services
            .AddOptions<ApplicationCommandServiceOptions>()
            .BindConfiguration("Discord")
            .BindConfiguration("Discord:ApplicationCommands");

        services
            .AddOptions<ApplicationCommandServiceOptions<TInteraction, TContext>>()
            .Configure<IOptions<ApplicationCommandServiceOptions>>((options, baseOptions) => options.Apply(baseOptions))
            .PostConfigure(configureOptions);

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<ApplicationCommandServiceOptions<TInteraction, TContext>>>().Value;
            return new ApplicationCommandService<TContext>(options.CreateConfiguration());
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

    public static IServiceCollection AddApplicationCommands<TInteraction,
                                                            [DAM(DAMT.PublicConstructors)] TContext,
                                                            [DAM(DAMT.PublicConstructors)] TAutocompleteContext>(
        this IServiceCollection services)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return services.AddApplicationCommands<TInteraction, TContext, TAutocompleteContext>((_, _) => { });
    }

    public static IServiceCollection AddApplicationCommands<TInteraction,
                                                            [DAM(DAMT.PublicConstructors)] TContext,
                                                            [DAM(DAMT.PublicConstructors)] TAutocompleteContext>(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return services.AddApplicationCommands<TInteraction, TContext, TAutocompleteContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddApplicationCommands<TInteraction,
                                                            [DAM(DAMT.PublicConstructors)] TContext,
                                                            [DAM(DAMT.PublicConstructors)] TAutocompleteContext>(
        this IServiceCollection services,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>, IServiceProvider> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        services
            .AddOptions<ApplicationCommandServiceOptions>()
            .BindConfiguration("Discord")
            .BindConfiguration("Discord:ApplicationCommands");

        services
            .AddOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>>()
            .Configure<IOptions<ApplicationCommandServiceOptions>>((options, baseOptions) => options.Apply(baseOptions))
            .PostConfigure(configureOptions);

        services.AddSingleton<IOptions<ApplicationCommandServiceOptions<TInteraction, TContext>>>(services => services.GetRequiredService<IOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>>>());

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>>>().Value;
            return new ApplicationCommandService<TContext, TAutocompleteContext>(options.CreateConfiguration());
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
