using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCord.Gateway;

namespace NetCord.Hosting.Gateway;

public static class ShardedGatewayClientHostBuilderExtensions
{
    public static IHostBuilder UseDiscordShardedGateway(this IHostBuilder builder)
    {
        return builder.UseDiscordShardedGateway((_, _) => { });
    }

    public static IHostBuilder UseDiscordShardedGateway(this IHostBuilder builder, Action<ShardedGatewayClientOptions> configureOptions)
    {
        return builder.UseDiscordShardedGateway((options, _) => configureOptions(options));
    }

    public static IHostBuilder UseDiscordShardedGateway(this IHostBuilder builder, Action<ShardedGatewayClientOptions, IServiceProvider> configureOptions)
    {
        builder.ConfigureServices((context, services) =>
        {
            services
                .AddOptions<ShardedGatewayClientOptions>()
                .BindConfiguration("Discord")
                .Configure(configureOptions)
                .ValidateDataAnnotations();

            services.AddSingleton<IOptions<IDiscordOptions>>(services => services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>());

            services.AddSingleton(services =>
            {
                var options = services.GetRequiredService<IOptions<ShardedGatewayClientOptions>>().Value;

                var token = ConfigurationHelper.ParseToken(options.Token!, services);

                if (token is not IEntityToken entityToken)
                    throw new InvalidOperationException($"Unable to initialize '{nameof(ShardedGatewayClient)}'. The provided token must implement the '{nameof(IEntityToken)}' interface.");

                return new ShardedGatewayClient(entityToken, options.Configuration);
            });
            services.AddSingleton(services => services.GetRequiredService<ShardedGatewayClient>().Rest);

            services.AddHostedService<ShardedGatewayClientHostedService>();
        });
        return builder;
    }
}
