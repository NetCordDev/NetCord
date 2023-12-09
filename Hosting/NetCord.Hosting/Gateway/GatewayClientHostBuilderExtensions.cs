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

                var token = ConfigurationHelper.ParseToken(options.Token!);
                return new GatewayClient(token, options.Configuration);
            });
            services.AddSingleton(services => services.GetRequiredService<GatewayClient>().Rest);

            services.AddHostedService<GatewayClientHostedService>();
        });
        return builder;
    }
}

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

                var token = ConfigurationHelper.ParseToken(options.Token!);
                return new ShardedGatewayClient(token, options.Configuration);
            });
            services.AddSingleton(services => services.GetRequiredService<ShardedGatewayClient>().Rest);

            services.AddHostedService<ShardedGatewayClientHostedService>();
        });
        return builder;
    }
}
