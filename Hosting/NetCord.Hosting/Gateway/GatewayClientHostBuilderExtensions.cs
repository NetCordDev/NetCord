using Microsoft.Extensions.Hosting;

namespace NetCord.Hosting.Gateway;

public static class GatewayClientHostBuilderExtensions
{
    // Configure

    public static IHostBuilder ConfigureDiscordGateway(this IHostBuilder builder, Action<GatewayClientOptions> configureOptions)
    {
        return builder.ConfigureDiscordGateway((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureDiscordGateway(this IHostBuilder builder, Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((_, services) => services.ConfigureDiscordGateway(configureOptions));
    }

    // Use

    public static IHostBuilder UseDiscordGateway(this IHostBuilder builder)
    {
        return builder.UseDiscordGateway((_, _) => { });
    }

    public static IHostBuilder UseDiscordGateway(this IHostBuilder builder, Action<GatewayClientOptions> configureOptions)
    {
        return builder.UseDiscordGateway((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseDiscordGateway(this IHostBuilder builder, Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.AddDiscordGateway(configureOptions));
    }
}
