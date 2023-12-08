using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public static class CommandServiceHostBuilderExtensions
{
    public static IHostBuilder UseCommandService<TContext>(this IHostBuilder builder)
        where TContext : ICommandContext
    {
        return builder.UseCommandService<TContext>((_, _) => { });
    }

    public static IHostBuilder UseCommandService<TContext>(this IHostBuilder builder,
                                                           Action<CommandServiceOptions<TContext>> configureOptions)
        where TContext : ICommandContext
    {
        return builder.UseCommandService<TContext>((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseCommandService<TContext>(this IHostBuilder builder,
                                                           Action<CommandServiceOptions<TContext>, IServiceProvider> configureOptions)
        where TContext : ICommandContext
    {
        builder.ConfigureServices((context, services) =>
        {
            services
                .AddOptions<CommandServiceOptions<TContext>>()
                .BindConfiguration("Discord")
                .Configure(configureOptions);

            services.AddSingleton(services =>
            {
                var options = services.GetRequiredService<IOptions<CommandServiceOptions<TContext>>>().Value;
                return new CommandService<TContext>(options.Configuration);
            });
            services.AddSingleton<IService>(services => services.GetRequiredService<CommandService<TContext>>());

            services.AddSingleton<CommandHandler<TContext>>();
            services.AddGatewayEventHandler(services => services.GetRequiredService<CommandHandler<TContext>>());
            services.AddShardedGatewayEventHandler(services => services.GetRequiredService<CommandHandler<TContext>>());
        });
        return builder;
    }
}
