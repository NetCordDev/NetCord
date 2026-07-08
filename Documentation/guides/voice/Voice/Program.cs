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

    VoiceClient? voiceClient;
    try
    {
        voiceClient = await context.Client.JoinVoiceChannelAsync(guildId, channelId, new()
        {
            Logger = new ConsoleLogger(),
        });
    }
    catch
    {
        voiceInstances.TryRemove(item: new(guildId, null));

        await context.Client.UpdateVoiceStateAsync(new(guildId, null));

        throw;
    }

    VoiceInstance voiceInstance = new(voiceClient);

    if (!voiceInstances.TryUpdate(guildId, voiceInstance, null))
    {
        // This should never happen with the example code,
        // but we'll handle it just in case someone modifies
        // it in a way that could cause it to happen

        voiceInstance.Dispose();

        await context.Client.UpdateVoiceStateAsync(new(guildId, null));

        return new InteractionMessageProperties()
            .WithContent("Failed to register voice connection.")
            .WithFlags(MessageFlags.Ephemeral);
    }

    voiceClient.Disconnect += args =>
    {
        if (args.Reconnect)
            return default;

        if (voiceInstances.TryRemove(item: new(guildId, voiceInstance)))
            voiceInstance.Dispose();

        return default;
    };

    try
    {
        await voiceClient.StartAsync();
    }
    catch
    {
        if (voiceInstances.TryRemove(item: new(guildId, voiceInstance)))
        {
            voiceInstance.Dispose();

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

    if (!voiceInstances.TryGetValue(guildId, out var voiceInstance) || voiceInstance is null)
        return new InteractionMessageProperties()
            .WithContent("Not connected to a voice channel in this guild.")
            .WithFlags(MessageFlags.Ephemeral);

    if (voiceInstances.TryRemove(item: new(guildId, voiceInstance)))
    {
        try
        {
            await voiceInstance.Client.CloseAsync();
        }
        finally
        {
            voiceInstance.Dispose();

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

    if (!voiceInstances.TryGetValue(guildId, out var voiceInstance) || voiceInstance is null)
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            .WithContent("Not connected to a voice channel in this guild.")
            .WithFlags(MessageFlags.Ephemeral)));
        return;
    }

    using var job = voiceInstance.TryEnterJob(VoiceJobType.Playing);
    if (job is not { CancellationToken: var cancellationToken })
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            .WithContent("Already playing audio in this guild.")
            .WithFlags(MessageFlags.Ephemeral)));
        return;
    }

    await context.Interaction.SendResponseAsync(InteractionCallback.Message($"Playing..."));

    var voiceClient = voiceInstance.Client;

    await voiceClient.EnterSpeakingStateAsync(new(SpeakingFlags.Microphone));

    using var voiceStream = voiceClient.CreateVoiceStream();
    using OpusEncodeStream opusEncodeStream = new(voiceStream,
                                                  PcmFormat.Float,
                                                  VoiceChannels.Stereo,
                                                  OpusApplication.Audio);

    const string Input = "https://preview.netcord.dev/359/sounds/sample.mp3"; // TODO: Change to netcord.dev

    using var ffmpeg = Process.Start(new ProcessStartInfo
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

    var ffmpegOutput = ffmpeg.StandardOutput.BaseStream;

    try
    {
        await ffmpegOutput.CopyToAsync(opusEncodeStream, cancellationToken);
        await opusEncodeStream.FlushAsync(cancellationToken);
    }
    catch (Exception ex)
    {
        ffmpeg.Kill();

        if (ex is not OperationCanceledException and not AggregateException { InnerException: OperationCanceledException })
            throw;
    }
}).AddContexts(InteractionContextType.Guild);

host.AddSlashCommand("record", "Records audio", async (ApplicationCommandContext context, User? user = null) =>
{
    if (context.Guild is not { } guild)
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            .WithContent("The guild is not available. Try again later.")
            .WithFlags(MessageFlags.Ephemeral)));
        return;
    }

    var guildId = guild.Id;

    if (!voiceInstances.TryGetValue(guildId, out var voiceInstance) || voiceInstance is null)
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            .WithContent("Not connected to a voice channel in this guild.")
            .WithFlags(MessageFlags.Ephemeral)));
        return;
    }

    using var job = voiceInstance.TryEnterJob(VoiceJobType.Recording);
    if (job is not { CancellationToken: var cancellationToken })
    {
        await context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties()
            .WithContent("Already recording audio in this guild.")
            .WithFlags(MessageFlags.Ephemeral)));
        return;
    }

    await context.Interaction.SendResponseAsync(InteractionCallback.Message($"Recording..."));

    var voiceClient = voiceInstance.Client;

    user ??= context.User;

    using var ffmpeg = Process.Start(new ProcessStartInfo
    {
        FileName = "ffmpeg",
        ArgumentList =
        {
            "-f", BitConverter.IsLittleEndian ? "f32le" : "f32be",
            "-ar", "48000",
            "-ac", "1",
            "-i", "pipe:0",
            "-f", "ogg",
            "-c:a", "libopus",
            "pipe:1",
        },
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
    })!;

    using var ffmpegInput = ffmpeg.StandardInput.BaseStream;
    using OpusDecodeStream opusDecodeStream = new(ffmpegInput, PcmFormat.Float, VoiceChannels.Mono);

    Func<VoiceReceiveEventArgs, ValueTask> voiceReceive = args =>
    {
        if (voiceClient.Cache.SsrcUsers.TryGetValue(args.Ssrc, out var userId) && userId == user.Id)
            opusDecodeStream.Write(args.Frame);

        return default;
    };

    voiceClient.VoiceReceive += voiceReceive;

    using var ffmpegOutput = ffmpeg.StandardOutput.BaseStream;

    using MemoryStream outputStream = new();

    string? recordingStopReason;

    var copyBuffer = ArrayPool<byte>.Shared.Rent(4096);

    var ffmpegRunning = true;

    const int MaxFileSize = 10 * 1024 * 1024; // 10 MB

    try
    {
        int count;
        while (true)
        {
            if ((count = await ffmpegOutput.ReadAsync(copyBuffer, cancellationToken)) is 0)
            {
                // Should never happen since Ffmpeg should be keept alive
                // until we kill it, but we'll handle it just in case

                ffmpegRunning = false;
                recordingStopReason = "End of stream";
                break;
            }

            outputStream.Write(copyBuffer.AsSpan(0, count));

            if (outputStream.Length >= MaxFileSize)
            {
                recordingStopReason = "Maximum file size exceeded";
                break;
            }
        }
    }
    catch (Exception ex)
    {
        if (ex is not OperationCanceledException and not AggregateException { InnerException: OperationCanceledException })
            throw;

        recordingStopReason = "Disconnected";
    }
    finally
    {
        voiceClient.VoiceReceive -= voiceReceive;

        if (ffmpegRunning)
        {
            // Flush the stream to ensure all data is written to Ffmpeg
            // This way we don't cut off the end of the recording
            await opusDecodeStream.FlushAsync();
            await opusDecodeStream.DisposeAsync();

            await ffmpegOutput.CopyToAsync(outputStream);

            ffmpeg.Kill();
        }

        ArrayPool<byte>.Shared.Return(copyBuffer);
    }

    // Ensure the output stream is not larger than the maximum file size
    if (outputStream.Length > MaxFileSize)
        outputStream.SetLength(MaxFileSize);

    outputStream.Position = 0;

    await context.Channel.SendMessageAsync(new MessageProperties()
        .WithContent($"Finished recording. Stop reason: {recordingStopReason}")
        .AddAttachments(new AttachmentProperties("recording.ogg", outputStream)));
}).AddContexts(InteractionContextType.Guild);

await host.RunAsync();
