using Microsoft.Extensions.Hosting;

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
        return builder.ConfigureServices((context, services) => services.AddApplicationCommandService(configureOptions));
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
        return builder.ConfigureServices((context, services) => services.AddApplicationCommandService(configureOptions));
    }
}
