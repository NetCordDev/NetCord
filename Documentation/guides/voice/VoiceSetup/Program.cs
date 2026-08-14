#pragma warning disable IDE0005

using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;

using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Logging;
using NetCord.Rest;
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
