using Microsoft.Extensions.Hosting;

using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Hosting.Gateway;

public static class ShardedGatewayClientHostBuilderExtensions
{
    // Configure

    /// <summary>
    /// Configures a <see cref="ShardedGatewayClient"/> in the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to configure the <see cref="ShardedGatewayClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ShardedGatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder ConfigureDiscordShardedGateway(
        this IHostBuilder builder,
        Action<ShardedGatewayClientOptions> configureOptions)
    {
        return builder.ConfigureDiscordShardedGateway((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Configures a <see cref="ShardedGatewayClient"/> in the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to configure the <see cref="ShardedGatewayClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ShardedGatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder ConfigureDiscordShardedGateway(
        this IHostBuilder builder,
        Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((_, services) => services.ConfigureDiscordShardedGateway(configureOptions));
    }

    // Use

    /// <summary>
    /// Adds a <see cref="ShardedGatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="ShardedGatewayClient"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordShardedGateway(
        this IHostBuilder builder)
    {
        return builder.UseDiscordShardedGateway((_, _) => { });
    }

    /// <summary>
    /// Adds a <see cref="ShardedGatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="ShardedGatewayClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ShardedGatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordShardedGateway(
        this IHostBuilder builder,
        Action<ShardedGatewayClientOptions> configureOptions)
    {
        return builder.UseDiscordShardedGateway((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Adds a <see cref="ShardedGatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="ShardedGatewayClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="ShardedGatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordShardedGateway(
        this IHostBuilder builder,
        Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.AddDiscordShardedGateway(configureOptions));
    }
}
