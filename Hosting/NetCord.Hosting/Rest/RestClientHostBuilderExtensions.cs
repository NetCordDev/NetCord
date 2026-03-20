using Microsoft.Extensions.Hosting;

using NetCord.Rest;

namespace NetCord.Hosting.Rest;

public static class RestClientHostBuilderExtensions
{
    // Configure

    /// <summary>
    /// Configures a <see cref="RestClient"/> in the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to configure the <see cref="RestClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="RestClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder ConfigureDiscordRest(
        this IHostBuilder builder,
        Action<RestClientOptions> configureOptions)
    {
        return builder.ConfigureDiscordRest((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Configures a <see cref="RestClient"/> in the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to configure the <see cref="RestClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="RestClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder ConfigureDiscordRest(
        this IHostBuilder builder,
        Action<RestClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((_, services) => services.ConfigureDiscordRest(configureOptions));
    }

    // Use

    /// <summary>
    /// Adds a <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="RestClient"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordRest(
        this IHostBuilder builder)
    {
        return builder.UseDiscordRest((_, _) => { });
    }

    /// <summary>
    /// Adds a <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="RestClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="RestClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordRest(
        this IHostBuilder builder,
        Action<RestClientOptions> configureOptions)
    {
        return builder.UseDiscordRest((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Adds a <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="RestClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="RestClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordRest(
        this IHostBuilder builder,
        Action<RestClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.AddDiscordRest(configureOptions));
    }
}
