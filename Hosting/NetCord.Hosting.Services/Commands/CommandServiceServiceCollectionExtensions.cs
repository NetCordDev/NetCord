using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public static class CommandServiceServiceCollectionExtensions
{
    public static IServiceCollection AddCommandService<TContext>(this IServiceCollection services)
        where TContext : ICommandContext
    {
        return services.AddCommandService<TContext>((_, _) => { });
    }

    public static IServiceCollection AddCommandService<TContext>(this IServiceCollection services,
                                                                      Action<CommandServiceOptions<TContext>> configureOptions)
        where TContext : ICommandContext
    {
        return services.AddCommandService<TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddCommandService<TContext>(this IServiceCollection services,
                                                                             Action<CommandServiceOptions<TContext>, IServiceProvider> configureOptions)
        where TContext : ICommandContext
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

        return services;
    }
}
