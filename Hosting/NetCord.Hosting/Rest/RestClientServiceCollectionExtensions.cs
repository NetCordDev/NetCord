using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.Rest;

public static class RestClientServiceCollectionExtensions
{
    // Configure

    public static IServiceCollection ConfigureDiscordRest(this IServiceCollection services, Action<RestClientOptions> configureOptions)
    {
        return services.ConfigureDiscordRest((options, _) => configureOptions(options));
    }

    public static IServiceCollection ConfigureDiscordRest(this IServiceCollection services, Action<RestClientOptions, IServiceProvider> configureOptions)
    {
        services
            .AddOptions<RestClientOptions>()
            .PostConfigure(configureOptions);

        return services;
    }

    // Add

    public static IServiceCollection AddDiscordRest(this IServiceCollection services)
    {
        return services.AddDiscordRest((_, _) => { });
    }

    public static IServiceCollection AddDiscordRest(this IServiceCollection services, Action<RestClientOptions> configureOptions)
    {
        return services.AddDiscordRest((options, _) => configureOptions(options));
    }

    public static IServiceCollection AddDiscordRest(this IServiceCollection services, Action<RestClientOptions, IServiceProvider> configureOptions)
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
            var configuration = options.CreateConfiguration();
            return token is null ? new(configuration) : new(ConfigurationHelper.ParseToken(token, services), configuration);
        });

        return services;
    }
}
