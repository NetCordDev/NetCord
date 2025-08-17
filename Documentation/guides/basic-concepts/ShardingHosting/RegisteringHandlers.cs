using Microsoft.Extensions.Hosting;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace MyBot;

internal class RegisteringHandlers
{
    public static async Task RegisterHandlersAsync(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services
            .AddDiscordShardedGateway(options =>
            {
                options.Intents = GatewayIntents.GuildMessages
                                  | GatewayIntents.DirectMessages
                                  | GatewayIntents.MessageContent;
            })
            .AddShardedGatewayHandlers(typeof(Program).Assembly);

        var host = builder.Build();

        await host.RunAsync();
    }
}
