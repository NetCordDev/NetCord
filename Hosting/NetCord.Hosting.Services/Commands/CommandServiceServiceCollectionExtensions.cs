﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Hosting.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Hosting.Services.Commands;

public static class CommandServiceServiceCollectionExtensions
{
    // Configure

    public static IServiceCollection ConfigureCommands(this IServiceCollection services,
                                                       Action<CommandServiceOptions> configureOptions)
    {
        return services.ConfigureCommands((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureCommands(this IServiceCollection services,
                                                       Action<CommandServiceOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<CommandServiceOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    public static IServiceCollection ConfigureCommands<TContext>(this IServiceCollection services,
                                                                 Action<CommandServiceOptions<TContext>> configureOptions)
        where TContext : ICommandContext
    {
        return services.ConfigureCommands<TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureCommands<TContext>(this IServiceCollection services,
                                                                 Action<CommandServiceOptions<TContext>, IServiceProvider> configureOptions)
        where TContext : ICommandContext
    {
        services
            .AddOptions<CommandServiceOptions<TContext>>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    public static IServiceCollection AddCommands<TContext>(this IServiceCollection services)
        where TContext : ICommandContext
    {
        return services.AddCommands<TContext>((_, _) => { });
    }

    public static IServiceCollection AddCommands<TContext>(this IServiceCollection services,
                                                           Action<CommandServiceOptions<TContext>> configureOptions)
        where TContext : ICommandContext
    {
        return services.AddCommands<TContext>((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddCommands<TContext>(this IServiceCollection services,
                                                           Action<CommandServiceOptions<TContext>, IServiceProvider> configureOptions)
        where TContext : ICommandContext
    {
        services
            .AddOptions<CommandServiceOptions<TContext>>()
            .BindConfiguration("Discord")
            .BindConfiguration("Discord:Commands")
            .Configure<IOptions<CommandServiceOptions>>((options, baseOptions) => options.Apply(baseOptions))
            .PostConfigure(configureOptions);

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<CommandServiceOptions<TContext>>>().Value;
            return new CommandService<TContext>(options.CreateConfiguration());
        });
        services.AddSingleton<IService>(services => services.GetRequiredService<CommandService<TContext>>());

        services.AddSingleton<CommandHandler<TContext>>();
        services.AddGatewayEventHandler(services => services.GetRequiredService<CommandHandler<TContext>>());
        services.AddShardedGatewayEventHandler(services => services.GetRequiredService<CommandHandler<TContext>>());

        return services;
    }
}
