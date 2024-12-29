using Microsoft.Extensions.Hosting;

using MyBot;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext>()
    .AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>()
    .AddCommands<CommandContext>();

var host = builder.Build();

host.AddSlashCommand(
        name: "hi",
        description: "Hi!",
        [RequireAnimatedAvatar<ApplicationCommandContext>] () => "Hi! You can use this command because your avatar is animated!");

host.AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();
