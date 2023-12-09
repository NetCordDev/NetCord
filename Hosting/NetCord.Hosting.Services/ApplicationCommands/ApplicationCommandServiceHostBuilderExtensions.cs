using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public static class ApplicationCommandServiceHostBuilderExtensions
{
    public static IHostBuilder UseApplicationCommandService<TInteraction, TContext>(this IHostBuilder builder)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return builder.UseApplicationCommandService<TInteraction, TContext>((_, _) => { });
    }

    public static IHostBuilder UseApplicationCommandService<TInteraction, TContext>(this IHostBuilder builder,
                                                                                    Action<ApplicationCommandServiceOptions<TInteraction, TContext>> configureOptions)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return builder.UseApplicationCommandService<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseApplicationCommandService<TInteraction, TContext>(this IHostBuilder builder,
                                                                                    Action<ApplicationCommandServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        builder.ConfigureServices((context, services) =>
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

            services.TryAddApplicationCommandServiceHostedService();
        });
        return builder;
    }

    public static IHostBuilder UseApplicationCommandService<TInteraction, TContext, TAutocompleteContext>(this IHostBuilder builder)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return builder.UseApplicationCommandService<TInteraction, TContext, TAutocompleteContext>((_, _) => { });
    }

    public static IHostBuilder UseApplicationCommandService<TInteraction, TContext, TAutocompleteContext>(this IHostBuilder builder,
                                                                                                          Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>> configureOptions)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return builder.UseApplicationCommandService<TInteraction, TContext, TAutocompleteContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseApplicationCommandService<TInteraction, TContext, TAutocompleteContext>(this IHostBuilder builder,
                                                                                                          Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>, IServiceProvider> configureOptions)
        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        builder.ConfigureServices((context, services) =>
        {
            services
                .AddOptions<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>>()
                .Configure(configureOptions);

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

            services.TryAddApplicationCommandServiceHostedService();
        });
        return builder;
    }

    private static void TryAddApplicationCommandServiceHostedService(this IServiceCollection services)
    {
        var count = services.Count;
        var type = typeof(ApplicationCommandServiceHostedService);
        for (var i = 0; i < count; i++)
            if (services[i].ImplementationType == type)
                return;

        services.AddHostedService<ApplicationCommandServiceHostedService>();
    }
}
