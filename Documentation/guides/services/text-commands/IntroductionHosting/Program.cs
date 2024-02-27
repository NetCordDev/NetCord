using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.Commands;
using NetCord.Services.Commands;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseCommands<CommandContext>();

var host = builder.Build()
    .AddCommand<CommandContext>(["ping"], () => "Pong!")
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();
