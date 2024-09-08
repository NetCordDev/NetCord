using Microsoft.Extensions.Hosting;

using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public static class CommandServiceHostBuilderExtensions
{
    public static IHostBuilder UseCommands<TContext>(this IHostBuilder builder)
        where TContext : ICommandContext
    {
        return builder.UseCommands<TContext>((_, _) => { });
    }

    public static IHostBuilder UseCommands<TContext>(this IHostBuilder builder,
                                                     Action<CommandServiceOptions<TContext>> configureOptions)
        where TContext : ICommandContext
    {
        return builder.UseCommands<TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseCommands<TContext>(this IHostBuilder builder,
                                                     Action<CommandServiceOptions<TContext>, IServiceProvider> configureOptions)
        where TContext : ICommandContext
    {
        return builder.ConfigureServices((context, services) => services.AddCommands(configureOptions));
    }
}
