using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddApplicationCommands<ApplicationCommandInteraction, ApplicationCommandContext>();

var host = builder.Build();

// Add commands using minimal APIs
host.AddSlashCommand("ping", "Ping!", () => "Pong!")
    .AddUserCommand("Username", (User user) => user.Username)
    .AddMessageCommand("Length", (RestMessage message) => message.Content.Length.ToString());

// Add commands from modules
host.AddModules(typeof(Program).Assembly);

// Add handlers to handle the commands
host.UseGatewayEventHandlers();

await host.RunAsync();
