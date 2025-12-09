using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;

// TODO: Complete sample code for voice basics
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordGateway();

var host = builder.Build();

await host.RunAsync();
