using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;

// TODO: Complete sample code for Generic Host approach
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordGateway();

var host = builder.Build();

await host.RunAsync();
