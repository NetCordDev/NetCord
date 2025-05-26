using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Hosting.Gateway;

public static class ShardedGatewayClientServiceCollectionExtensions
{
    // Configure

    /// <summary>
    /// Configures a <see cref="ShardedGatewayClient"/> in the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure the <see cref="ShardedGatewayClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ShardedGatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection ConfigureDiscordShardedGateway(
        this IServiceCollection services,
        Action<ShardedGatewayClientOptions> configureOptions)
    {
        return services.ConfigureDiscordShardedGateway((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Configures a <see cref="ShardedGatewayClient"/> in the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure the <see cref="ShardedGatewayClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ShardedGatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection ConfigureDiscordShardedGateway(
        this IServiceCollection services,
        Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<ShardedGatewayClientOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    /// <summary>
    /// Adds a <see cref="ShardedGatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="ShardedGatewayClient"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordShardedGateway(
        this IServiceCollection services)
    {
        return services.AddDiscordShardedGateway((_, _) => { });
    }

    /// <summary>
    /// Adds a <see cref="ShardedGatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="ShardedGatewayClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ShardedGatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordShardedGateway(
        this IServiceCollection services,
        Action<ShardedGatewayClientOptions> configureOptions)
    {
        return services.AddDiscordShardedGateway((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Adds a <see cref="ShardedGatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="ShardedGatewayClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ShardedGatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordShardedGateway(
        this IServiceCollection services,
        Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        services.AddSingleton<IValidateOptions<ShardedGatewayClientOptions>, ShardedGatewayClientOptions.Validator>();

        services
            .AddOptions<ShardedGatewayClientOptions>()
            .BindConfiguration("Discord")
            .PostConfigure(configureOptions);

        services.AddSingleton<IOptions<IDiscordOptions>>(services => services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>());

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>().Value;

            var token = ConfigurationHelper.ParseToken(options.Token!, services);

            if (token is not IEntityToken entityToken)
                throw new InvalidOperationException($"Unable to initialize '{nameof(ShardedGatewayClient)}'. The provided token must implement the '{nameof(IEntityToken)}' interface.");

            return new ShardedGatewayClient(entityToken, options.CreateConfiguration(services));
        });
        services.AddSingleton(services => services.GetRequiredService<ShardedGatewayClient>().Rest);

        services.AddHostedService<ShardedGatewayClientHostedService>();

        return services;
    }
}
