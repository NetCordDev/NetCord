using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordShardedGateway();

var host = builder.Build();

await host.RunAsync();
