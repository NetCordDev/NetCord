using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.Commands;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddCommands();

var host = builder.Build();

// Add a command using minimal APIs
host.AddCommand(["pong"], () => "Ping!");

// Add commands from modules
host.AddModules(typeof(Program).Assembly);

await host.RunAsync();
