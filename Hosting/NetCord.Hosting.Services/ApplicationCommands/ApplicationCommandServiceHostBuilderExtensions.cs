using Microsoft.Extensions.Hosting;

using NetCord.Services.ApplicationCommands;

namespace NetCord.Hosting.Services.ApplicationCommands;

public static class ApplicationCommandServiceHostBuilderExtensions
{
    // Configure

    public static IHostBuilder ConfigureApplicationCommands(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions> configureOptions)
    {
        return builder.ConfigureApplicationCommands((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureApplicationCommands(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.ConfigureApplicationCommands(configureOptions));
    }

    public static IHostBuilder ConfigureApplicationCommands<TInteraction,
                                                            TContext>(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext>> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return builder.ConfigureApplicationCommands<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureApplicationCommands<TInteraction, TContext>(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return builder.ConfigureServices((context, services) => services.ConfigureApplicationCommands(configureOptions));
    }

    public static IHostBuilder ConfigureApplicationCommands<TInteraction,
                                                            TContext,
                                                            TAutocompleteContext>(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return builder.ConfigureApplicationCommands<TInteraction, TContext, TAutocompleteContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureApplicationCommands<TInteraction,
                                                            TContext,
                                                            TAutocompleteContext>(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>, IServiceProvider> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return builder.ConfigureServices((context, services) => services.ConfigureApplicationCommands(configureOptions));
    }

    // Use

    public static IHostBuilder UseApplicationCommands(
        this IHostBuilder builder)
    {
        return builder.UseApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>((_, _) => { });
    }

    public static IHostBuilder UseApplicationCommands(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>> configureOptions)
    {
        return builder.UseApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseApplicationCommands(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>, IServiceProvider> configureOptions)
    {
        return builder.UseApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext, AutocompleteInteractionContext>(configureOptions);
    }

    public static IHostBuilder UseHttpApplicationCommands(
        this IHostBuilder builder)
    {
        return builder.UseApplicationCommands<ApplicationCommandInteraction, HttpApplicationCommandContext, HttpAutocompleteInteractionContext>((_, _) => { });
    }

    public static IHostBuilder UseHttpApplicationCommands(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<ApplicationCommandInteraction, HttpApplicationCommandContext, HttpAutocompleteInteractionContext>> configureOptions)
    {
        return builder.UseApplicationCommands<ApplicationCommandInteraction, HttpApplicationCommandContext, HttpAutocompleteInteractionContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseHttpApplicationCommands(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<ApplicationCommandInteraction, HttpApplicationCommandContext, HttpAutocompleteInteractionContext>, IServiceProvider> configureOptions)
    {
        return builder.UseApplicationCommands(configureOptions);
    }

    public static IHostBuilder UseApplicationCommands<TInteraction,
                                                      [DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return builder.UseApplicationCommands<TInteraction, TContext>((_, _) => { });
    }

    public static IHostBuilder UseApplicationCommands<TInteraction,
                                                      [DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext>> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return builder.UseApplicationCommands<TInteraction, TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseApplicationCommands<TInteraction,
                                                      [DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext>, IServiceProvider> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
    {
        return builder.ConfigureServices((context, services) => services.AddApplicationCommands(configureOptions));
    }

    public static IHostBuilder UseApplicationCommands<TInteraction,
                                                      [DAM(DAMT.PublicConstructors)] TContext,
                                                      [DAM(DAMT.PublicConstructors)] TAutocompleteContext>(
        this IHostBuilder builder)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return builder.UseApplicationCommands<TInteraction, TContext, TAutocompleteContext>((_, _) => { });
    }

    public static IHostBuilder UseApplicationCommands<TInteraction,
                                                      [DAM(DAMT.PublicConstructors)] TContext,
                                                      [DAM(DAMT.PublicConstructors)] TAutocompleteContext>(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return builder.UseApplicationCommands<TInteraction, TContext, TAutocompleteContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseApplicationCommands<TInteraction,
                                                      [DAM(DAMT.PublicConstructors)] TContext,
                                                      [DAM(DAMT.PublicConstructors)] TAutocompleteContext>(
        this IHostBuilder builder,
        Action<ApplicationCommandServiceOptions<TInteraction, TContext, TAutocompleteContext>, IServiceProvider> configureOptions)

        where TInteraction : ApplicationCommandInteraction
        where TContext : IApplicationCommandContext
        where TAutocompleteContext : IAutocompleteInteractionContext
    {
        return builder.ConfigureServices((context, services) => services.AddApplicationCommands(configureOptions));
    }
}
