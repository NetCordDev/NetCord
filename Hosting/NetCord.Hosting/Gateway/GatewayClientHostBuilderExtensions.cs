using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static class GatewayClientHostBuilderExtensions
{
    public static IHostBuilder UseDiscordGateway(this IHostBuilder builder)
    {
        return builder.UseDiscordGateway((_, _) => { });
    }

    public static IHostBuilder UseDiscordGateway(this IHostBuilder builder, Action<GatewayClientOptions> configureOptions)
    {
        return builder.UseDiscordGateway((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseDiscordGateway(this IHostBuilder builder, Action<GatewayClientOptions, IServiceProvider> configureOptions)
    {
        builder.ConfigureServices((context, services) =>
        {
            services
                .AddOptions<GatewayClientOptions>()
                .BindConfiguration("Discord")
                .Configure(configureOptions)
                .ValidateDataAnnotations();

            services.AddSingleton<IOptions<IDiscordOptions>>(services => services.GetRequiredService<IOptions<GatewayClientOptions>>());

            services.AddSingleton(services =>
            {
                var options = services.GetRequiredService<IOptions<GatewayClientOptions>>().Value;

                var token = ConfigurationHelper.ParseToken(options.Token!, services);

                if (token is not IEntityToken entityToken)
                    throw new InvalidOperationException($"Unable to initialize '{nameof(GatewayClient)}'. The provided token must implement the '{nameof(IEntityToken)}' interface.");

                return new GatewayClient(entityToken, options.Configuration);
            });
            services.AddSingleton(services => services.GetRequiredService<GatewayClient>().Rest);

            services.AddHostedService<GatewayClientHostedService>();
        });
        return builder;
    }
}
