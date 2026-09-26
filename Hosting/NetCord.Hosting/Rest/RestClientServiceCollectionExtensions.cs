using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.Rest;

public static class RestClientServiceCollectionExtensions
{
    // Configure

    /// <summary>
    /// Configures a <see cref="RestClient"/> in the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure the <see cref="RestClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="RestClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection ConfigureDiscordRest(
        this IServiceCollection services,
        Action<RestClientOptions> configureOptions)
    {
        return services.ConfigureDiscordRest((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Configures a <see cref="RestClient"/> in the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure the <see cref="RestClient"/> on.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="RestClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection ConfigureDiscordRest(
        this IServiceCollection services,
        Action<RestClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<RestClientOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    /// <summary>
    /// Adds a <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="RestClient"/> to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordRest(
        this IServiceCollection services)
    {
        return services.AddDiscordRest((_, _) => { });
    }

    /// <summary>
    /// Adds a <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="RestClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="RestClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordRest(
        this IServiceCollection services,
        Action<RestClientOptions> configureOptions)
    {
        return services.AddDiscordRest((options, _) => configureOptions(options));
    }

    /// <summary>
    /// Adds a <see cref="RestClient"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="RestClient"/> to.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="RestClient"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddDiscordRest(
        this IServiceCollection services,
        Action<RestClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<RestClientOptions>()
            .BindConfiguration("Discord")
            .PostConfigure(configureOptions);

        services.AddSingleton<IOptions<IDiscordOptions>>(services => services.GetRequiredService<IOptions<RestClientOptions>>());

        services.AddSingleton<RestClient>(services =>
        {
            var options = services.GetRequiredService<IOptions<RestClientOptions>>().Value;

            var token = options.Token;
            var configuration = options.CreateConfiguration(services);
            return token is null ? new(configuration) : new(ConfigurationHelper.ParseToken(token, services), configuration);
        });

        return services;
    }
}
