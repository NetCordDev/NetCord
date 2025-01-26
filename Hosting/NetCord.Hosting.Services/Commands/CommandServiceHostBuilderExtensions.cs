using Microsoft.Extensions.Hosting;

using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public static class CommandServiceHostBuilderExtensions
{
    // Configure

    public static IHostBuilder ConfigureCommands(
        this IHostBuilder builder,
        Action<CommandServiceOptions> configureOptions)
    {
        return builder.ConfigureCommands((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureCommands(
        this IHostBuilder builder,
        Action<CommandServiceOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.ConfigureCommands(configureOptions));
    }

    public static IHostBuilder ConfigureCommands<TContext>(
        this IHostBuilder builder,
        Action<CommandServiceOptions<TContext>> configureOptions)

        where TContext : ICommandContext
    {
        return builder.ConfigureCommands<TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureCommands<TContext>(
        this IHostBuilder builder,
        Action<CommandServiceOptions<TContext>, IServiceProvider> configureOptions)

        where TContext : ICommandContext
    {
        return builder.ConfigureServices((context, services) => services.ConfigureCommands(configureOptions));
    }

    // Use

    public static IHostBuilder UseCommands(
        this IHostBuilder builder)
    {
        return builder.UseCommands<CommandContext>((_, _) => { });
    }

    public static IHostBuilder UseCommands(
        this IHostBuilder builder,
        Action<CommandServiceOptions<CommandContext>> configureOptions)
    {
        return builder.UseCommands<CommandContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseCommands(
        this IHostBuilder builder,
        Action<CommandServiceOptions<CommandContext>, IServiceProvider> configureOptions)
    {
        return builder.UseCommands<CommandContext>(configureOptions);
    }

    public static IHostBuilder UseCommands<[DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder)

        where TContext : ICommandContext
    {
        return builder.UseCommands<TContext>((_, _) => { });
    }

    public static IHostBuilder UseCommands<[DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder,
        Action<CommandServiceOptions<TContext>> configureOptions)

        where TContext : ICommandContext
    {
        return builder.UseCommands<TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseCommands<[DAM(DAMT.PublicConstructors)] TContext>(
        this IHostBuilder builder,
        Action<CommandServiceOptions<TContext>, IServiceProvider> configureOptions)

        where TContext : ICommandContext
    {
        return builder.ConfigureServices((context, services) => services.AddCommands(configureOptions));
    }
}
