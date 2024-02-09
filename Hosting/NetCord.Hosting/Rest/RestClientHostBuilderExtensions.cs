using Microsoft.Extensions.Hosting;

namespace NetCord.Hosting.Rest;

public static class RestClientHostBuilderExtensions
{
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
