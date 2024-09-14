using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static class GatewayClientServiceCollectionExtensions
{
    // Configure

    public static IServiceCollection ConfigureDiscordGateway(this IServiceCollection services, Action<GatewayClientOptions> configureOptions)
    {
        return services.ConfigureDiscordGateway((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureDiscordGateway(this IServiceCollection services,
                                                             Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<GatewayClientOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    public static IServiceCollection AddDiscordGateway(this IServiceCollection services)
    {
        return services.AddDiscordGateway((_, _) => { });
    }

    public static IServiceCollection AddDiscordGateway(this IServiceCollection services, Action<GatewayClientOptions> configureOptions)
    {
        return services.AddDiscordGateway((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddDiscordGateway(this IServiceCollection services, Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<GatewayClientOptions>()
            .BindConfiguration("Discord")
            .PostConfigure(configureOptions)
            .ValidateDataAnnotations();

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
