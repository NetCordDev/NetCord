using Microsoft.Extensions.Hosting;

namespace NetCord.Hosting.Gateway;

public static class ShardedGatewayClientHostBuilderExtensions
{
    // Configure

    public static IHostBuilder ConfigureDiscordShardedGateway(this IHostBuilder builder, Action<ShardedGatewayClientOptions> configureOptions)
    {
        return builder.ConfigureDiscordShardedGateway((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureDiscordShardedGateway(this IHostBuilder builder, Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((_, services) => services.ConfigureDiscordShardedGateway(configureOptions));
    }

    // Use

    public static IHostBuilder UseDiscordShardedGateway(this IHostBuilder builder)
    {
        return builder.UseDiscordShardedGateway((_, _) => { });
    }

    public static IHostBuilder UseDiscordShardedGateway(this IHostBuilder builder, Action<ShardedGatewayClientOptions> configureOptions)
    {
        return builder.UseDiscordShardedGateway((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseDiscordShardedGateway(this IHostBuilder builder, Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.AddDiscordShardedGateway(configureOptions));
    }
}
