using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static class ShardedGatewayClientServiceCollectionExtensions
{
    // Configure

    public static IServiceCollection ConfigureDiscordShardedGateway(this IServiceCollection services, Action<ShardedGatewayClientOptions> configureOptions)
    {
        return services.ConfigureDiscordShardedGateway((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureDiscordShardedGateway(this IServiceCollection services, Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<ShardedGatewayClientOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    public static IServiceCollection AddDiscordShardedGateway(this IServiceCollection services)
    {
        return services.AddDiscordShardedGateway((_, _) => { });
    }

    public static IServiceCollection AddDiscordShardedGateway(this IServiceCollection services, Action<ShardedGatewayClientOptions> configureOptions)
    {
        return services.AddDiscordShardedGateway((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddDiscordShardedGateway(this IServiceCollection services, Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<ShardedGatewayClientOptions>()
            .BindConfiguration("Discord")
            .PostConfigure(configureOptions)
            .ValidateDataAnnotations();

        services.AddSingleton<IOptions<IDiscordOptions>>(services => services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>());

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>().Value;

            var token = ConfigurationHelper.ParseToken(options.Token!, services);

            if (token is not IEntityToken entityToken)
                throw new InvalidOperationException($"Unable to initialize '{nameof(ShardedGatewayClient)}'. The provided token must implement the '{nameof(IEntityToken)}' interface.");

            return new ShardedGatewayClient(entityToken, options.CreateConfiguration());
        });
        services.AddSingleton(services => services.GetRequiredService<ShardedGatewayClient>().Rest);

        services.AddHostedService<ShardedGatewayClientHostedService>();

        return services;
    }
}
