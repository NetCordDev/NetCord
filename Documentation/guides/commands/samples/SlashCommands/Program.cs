using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;

// TODO: Complete sample code for slash commands
var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddApplicationCommands<SlashCommandInteraction, SlashCommandContext>();

var host = builder.Build();

await host.RunAsync();
