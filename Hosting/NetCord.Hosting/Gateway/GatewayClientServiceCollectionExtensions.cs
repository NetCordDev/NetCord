using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Hosting.Gateway;

public static class GatewayClientServiceCollectionExtensions
{
    // Configure

    /// <summary>
    /// Configures a <see cref="GatewayClient"/> in the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure the <see cref="GatewayClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="GatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection ConfigureDiscordGateway(
        this IServiceCollection services,
        Action<GatewayClientOptions> configureOptions)
    {
        return services.ConfigureDiscordGateway((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Configures a <see cref="GatewayClient"/> in the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure the <see cref="GatewayClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="GatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection ConfigureDiscordGateway(
        this IServiceCollection services,
        Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<GatewayClientOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    /// <summary>
    /// Adds a <see cref="GatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="GatewayClient"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordGateway(
        this IServiceCollection services)
    {
        return services.AddDiscordGateway((_, _) => { });
    }

    /// <summary>
    /// Adds a <see cref="GatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="GatewayClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="GatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordGateway(
        this IServiceCollection services,
        Action<GatewayClientOptions> configureOptions)
    {
        return services.AddDiscordGateway((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Adds a <see cref="GatewayClient"/> and its associated <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="GatewayClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="GatewayClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordGateway(
        this IServiceCollection services,
        Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        services.AddSingleton<IValidateOptions<GatewayClientOptions>, GatewayClientOptions.Validator>();

        services
            .AddOptions<GatewayClientOptions>()
            .BindConfiguration("Discord")
            .PostConfigure(configureOptions);

        services.AddSingleton<IOptions<IDiscordOptions>>(services => services.GetRequiredService<IOptions<GatewayClientOptions>>());

        services.AddSingleton(services =>
        {
            var options = services.GetRequiredService<IOptions<GatewayClientOptions>>().Value;

            var token = ConfigurationHelper.ParseToken(options.Token!, services);

            if (token is not IEntityToken entityToken)
                throw new InvalidOperationException($"Unable to initialize '{nameof(GatewayClient)}'. The provided token must implement the '{nameof(IEntityToken)}' interface.");

            return new GatewayClient(entityToken, options.CreateConfiguration());
        });
        services.AddSingleton(services => services.GetRequiredService<GatewayClient>().Rest);

        services.AddHostedService<GatewayClientHostedService>();

        return services;
    }
}
