using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using NetCord.Rest;

namespace NetCord.Hosting.Rest;

public static class RestClientServiceCollectionExtensions
{
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
            .Configure(configureOptions);

        services.AddSingleton<IOptions<IDiscordOptions>>(services => services.GetRequiredService<IOptions<RestClientOptions>>());

        services.AddSingleton<RestClient>(services =>
        {
            var options = services.GetRequiredService<IOptions<RestClientOptions>>().Value;

            var token = options.Token;
            return token is null ? new(options.Configuration) : new(ConfigurationHelper.ParseToken(token, services), options.Configuration);
        });

        return services;
    }
}
