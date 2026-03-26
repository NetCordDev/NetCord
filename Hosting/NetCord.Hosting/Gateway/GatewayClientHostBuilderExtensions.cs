using Microsoft.Extensions.Hosting;

using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Hosting.Gateway;

public static class GatewayClientHostBuilderExtensions
{
    // Configure

    /// <summary>
    /// Configures a <see cref="GatewayClient"/> in the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to configure the <see cref="GatewayClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="GatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder ConfigureDiscordGateway(
        this IHostBuilder builder,
        Action<GatewayClientOptions> configureOptions)
    {
        return builder.ConfigureDiscordGateway((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Configures a <see cref="GatewayClient"/> in the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to configure the <see cref="GatewayClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="GatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder ConfigureDiscordGateway(
        this IHostBuilder builder,
        Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((_, services) => services.ConfigureDiscordGateway(configureOptions));
    }

    // Use

    /// <summary>
    /// Adds a <see cref="GatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="GatewayClient"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordGateway(
        this IHostBuilder builder)
    {
        return builder.UseDiscordGateway((_, _) => { });
    }

    /// <summary>
    /// Adds a <see cref="GatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="GatewayClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="GatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordGateway(
        this IHostBuilder builder,
        Action<GatewayClientOptions> configureOptions)
    {
        return builder.UseDiscordGateway((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Adds a <see cref="GatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> to add the <see cref="GatewayClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="GatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IHostBuilder UseDiscordGateway(
        this IHostBuilder builder,
        Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        return builder.ConfigureServices((context, services) => services.AddDiscordGateway(configureOptions));
    }
}
