using Microsoft.Extensions.Hosting;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway(options =>
    {
        options.Configuration = new()
        {
            Intents = GatewayIntents.GuildMessages
                      | GatewayIntents.DirectMessages
                      | GatewayIntents.MessageContent
                      | GatewayIntents.DirectMessageReactions
                      | GatewayIntents.GuildMessageReactions,
        };
    })
    .ConfigureServices(services => services.AddGatewayEventHandlers(typeof(Program).Assembly));

var host = builder.Build()
    .UseGatewayEventHandlers();

await host.RunAsync();
