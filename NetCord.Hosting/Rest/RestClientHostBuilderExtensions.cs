using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCord.Rest;

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
        builder.ConfigureServices((context, services) =>
        {
            services
                .AddOptions<RestClientOptions>()
                .BindConfiguration("Discord")
                .Configure(configureOptions);

            services.AddSingleton<IOptions<IDiscordOptions>>(services => services.GetRequiredService<IOptions<RestClientOptions>>());

            services.AddSingleton<RestClient>(services =>
            {
                var options = services.GetRequiredService<IOptions<RestClientOptions>>().Value;

                var tokenString = options.Token;
                return tokenString is null ? new(options.Configuration) : new(ConfigurationHelper.ParseToken(tokenString), options.Configuration);
            });
        });
        return builder;
    }
}
