#pragma warning disable IDE0005

using System.Collections.Concurrent;

using Microsoft.Extensions.Hosting;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplicationCommands(o =>
    {
        o.ResultHandler = ApplicationCommandResultHandler<ApplicationCommandContext>.Ephemeral;
    })
    .AddDiscordGateway(o => o.Intents = GatewayIntents.Guilds | GatewayIntents.GuildVoiceStates);

var host = builder.Build();

ConcurrentDictionary<ulong, VoiceInstance?> voiceInstances = [];

await host.RunAsync();
