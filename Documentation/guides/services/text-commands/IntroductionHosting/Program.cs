using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.Commands;
using NetCord.Services.Commands;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseCommandService<CommandContext>();

var host = builder.Build()
    // Before C# 12 you have to use 'new string[] { "ping" }' instead of '["ping"]'
    .AddCommand<CommandContext>(["ping"], () => "Pong!")
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();
