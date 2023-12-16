using Microsoft.Extensions.Hosting;

using NetCord.Hosting.Gateway;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordShardedGateway();

var host = builder.Build();

await host.RunAsync();
