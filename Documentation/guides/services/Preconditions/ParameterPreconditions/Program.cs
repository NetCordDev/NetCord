using Microsoft.Extensions.Hosting;

using MyBot;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext>()
    .AddCommands<CommandContext>();

var host = builder.Build();

host.AddCommand(
        aliases: ["bye"],
        ([MustContain<CommandContext>("bye")] string text) => text);

host.AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();
