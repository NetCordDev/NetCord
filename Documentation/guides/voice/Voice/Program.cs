using System.Collections.Concurrent;
using System.Diagnostics;

using Microsoft.Extensions.Hosting;

using NetCord;
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
        o.ResultHandler = new ApplicationCommandResultHandler<ApplicationCommandContext>(MessageFlags.Ephemeral);
    })
    .AddDiscordGateway();

var host = builder.Build();

ConcurrentDictionary<ulong, VoiceClient?> voiceInstances = [];

host.AddSlashCommand("join", "Joins a channel", async (ApplicationCommandContext context, IVoiceGuildChannel? channel = null) =>
{
    if (context.Guild is not { } guild)
        return new InteractionMessageProperties()
            .WithContent("The guild is not available. Try again later.")
            .WithFlags(MessageFlags.Ephemeral);

    var user = context.User;

    ulong channelId;
    if (channel is not null)
        channelId = channel.Id;
    else if (guild.VoiceStates.TryGetValue(user.Id, out var voiceState))
        channelId = voiceState.ChannelId.GetValueOrDefault();
    else
        return new InteractionMessageProperties()
            .WithContent("You must specify a channel or be connected to a voice channel.")
            .WithFlags(MessageFlags.Ephemeral);

    var guildId = guild.Id;

    if (!voiceInstances.TryAdd(guildId, null))
        return new InteractionMessageProperties()
            .WithContent("Already connected to a voice channel in this guild.")
            .WithFlags(MessageFlags.Ephemeral);

    VoiceClient? voiceClient = null;
    try
    {
        voiceClient = await context.Client.JoinVoiceChannelAsync(guildId, channelId, new()
        {
            ReceiveHandler = new VoiceReceiveHandler(),
            Logger = new ConsoleLogger(),
        });
    }
    catch
    {
        voiceInstances.TryRemove(item: new(guildId, null));

        voiceClient?.Dispose();

        await context.Client.UpdateVoiceStateAsync(new(guildId, null));

        throw;
    }

    if (!voiceInstances.TryUpdate(guildId, voiceClient, null))
    {
        // This should never happen with the example code,
        // but we'll handle it just in case someone modifies
        // it in a way that could cause it to happen

        voiceClient.Dispose();

        await context.Client.UpdateVoiceStateAsync(new(guildId, null));

        return new InteractionMessageProperties()
            .WithContent("Failed to register voice connection.")
            .WithFlags(MessageFlags.Ephemeral);
    }

    voiceClient.Disconnect += args =>
    {
        if (args.Reconnect)
            return default;

        if (voiceInstances.TryRemove(item: new(guildId, voiceClient)))
            voiceClient.Dispose();

        return default;
    };

    try
    {
        await voiceClient.StartAsync();
    }
    catch
    {
        if (voiceInstances.TryRemove(item: new(guildId, voiceClient)))
        {
            voiceClient.Dispose();

            await context.Client.UpdateVoiceStateAsync(new(guildId, null));
        }

        throw;
    }

    return "Joined voice channel.";
}).AddContexts(InteractionContextType.Guild);

host.AddSlashCommand("leave", "Leaves the voice channel", async (ApplicationCommandContext context) =>
{
    if (context.Guild is not { } guild)
        return new InteractionMessageProperties()
            .WithContent("The guild is not available. Try again later.")
            .WithFlags(MessageFlags.Ephemeral);

    var guildId = guild.Id;

    if (!voiceInstances.TryGetValue(guildId, out var voiceClient) || voiceClient is null)
        return new InteractionMessageProperties()
            .WithContent("Not connected to a voice channel in this guild.")
            .WithFlags(MessageFlags.Ephemeral);

    if (voiceInstances.TryRemove(item: new(guildId, voiceClient)))
    {
        try
        {
            await voiceClient.CloseAsync();
        }
        finally
        {
            voiceClient.Dispose();

            await context.Client.UpdateVoiceStateAsync(new(guildId, null));
        }
    }

    return "Left voice channel.";
}).AddContexts(InteractionContextType.Guild);

host.AddSlashCommand("play", "Plays audio", async (ApplicationCommandContext context) =>
{
    if (context.Guild is not { } guild)
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            .WithContent("The guild is not available. Try again later.")
            .WithFlags(MessageFlags.Ephemeral)));
        return;
    }

    var guildId = guild.Id;

    if (!voiceInstances.TryGetValue(guildId, out var voiceClient) || voiceClient is null)
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            .WithContent("Not connected to a voice channel in this guild.")
            .WithFlags(MessageFlags.Ephemeral)));
        return;
    }

    await context.Interaction.SendResponseAsync(InteractionCallback.Message($"Playing..."));

    await voiceClient.EnterSpeakingStateAsync(new(SpeakingFlags.Microphone));

    using var voiceStream = voiceClient.CreateVoiceStream();
    using OpusEncodeStream opusEncodeStream = new(voiceStream,
                                                  PcmFormat.Float,
                                                  VoiceChannels.Stereo,
                                                  OpusApplication.Audio);

    const string Input = "..."; // TODO: Replace with a link to an actual audio file

    using var process = Process.Start(new ProcessStartInfo
    {
        FileName = "ffmpeg",
        ArgumentList =
        {
            "-i", Input,
            "-f", BitConverter.IsLittleEndian ? "f32le" : "f32be",
            "-ar", "48000",
            "-ac", "2",
            "pipe:1",
        },
        RedirectStandardOutput = true,
    })!;

    try
    {
        var ffmpegOutput = process.StandardOutput.BaseStream;

        await ffmpegOutput.CopyToAsync(opusEncodeStream);
        await opusEncodeStream.FlushAsync();
    }
    catch
    {
        process.Kill();
    }
}).AddContexts(InteractionContextType.Guild);

await host.RunAsync();
