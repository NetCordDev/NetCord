using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordShardedGateway(o => o.Configuration = new()
    {
        ShardCount = 2,
        IntentsFactory = _ => GatewayIntents.All,
    })
    .UseApplicationCommands<SlashCommandInteraction, SlashCommandContext>()
    .UseCommands<CommandContext>();

var host = builder.Build();

host
    .AddSlashCommand<SlashCommandContext>("ping", "Ping!", (SlashCommandContext context) => "Pong!")
    .AddCommand<CommandContext>(["ping"], () => "Pong!")
    .UseGatewayEventHandlers();

await host.RunAsync();
