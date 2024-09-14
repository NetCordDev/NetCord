using Microsoft.Extensions.Hosting;

namespace NetCord.Hosting.Rest;

public static class RestClientHostBuilderExtensions
{
    // Configure

    public static IHostBuilder ConfigureDiscordRest(this IHostBuilder builder, Action<RestClientOptions> configureOptions)
    {
        return builder.ConfigureDiscordRest((options, _) => configureOptions(options));
    }

    public static IHostBuilder ConfigureDiscordRest(this IHostBuilder builder, Action<RestClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((_, services) => services.ConfigureDiscordRest(configureOptions));
    }

    // Use

    public static IHostBuilder UseDiscordRest(this IHostBuilder builder)
    {
        return builder.UseDiscordRest((options, _) => { });
    }

    public static IHostBuilder UseDiscordRest(this IHostBuilder builder, Action<RestClientOptions> configureOptions)
    {
        return builder.UseDiscordRest((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseDiscordRest(this IHostBuilder builder, Action<RestClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.AddDiscordRest(configureOptions));
    }
}
