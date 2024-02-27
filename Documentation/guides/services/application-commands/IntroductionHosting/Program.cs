using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseApplicationCommands<SlashCommandInteraction, SlashCommandContext>()
    .UseApplicationCommands<UserCommandInteraction, UserCommandContext>()
    .UseApplicationCommands<MessageCommandInteraction, MessageCommandContext>();

var host = builder.Build()
    .AddSlashCommand<SlashCommandContext>("ping", "Ping!", () => "Pong!")
    .AddUserCommand<UserCommandContext>("Username", (UserCommandContext context) => context.Target.Username)
    .AddMessageCommand<MessageCommandContext>("Length", (MessageCommandContext context) => context.Target.Content.Length.ToString())
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();
