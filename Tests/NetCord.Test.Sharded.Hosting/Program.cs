using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplicationCommands<SlashCommandInteraction, SlashCommandContext>()
    .AddCommands<CommandContext>()
    .ConfigureDiscordShardedGateway(o => o.Presence = new(UserStatusType.Idle))
    .AddDiscordShardedGateway()
    .AddShardedGatewayEventHandler<Message>(nameof(GatewayClient.MessageCreate), (Message message, GatewayClient client, ILogger<Message> logger) => logger.LogInformation(new EventId(client.Shard.GetValueOrDefault().Id), "Content: {}", message.Content));

var host = builder.Build();

host.AddSlashCommand("ping", "Ping!", (SlashCommandContext context) => "Pong!")
    .AddCommand(["ping"], () => "Pong!")
    .UseShardedGatewayEventHandlers();

await host.RunAsync();
