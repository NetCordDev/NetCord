using System.ComponentModel;
using System.Diagnostics;

using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Gateway.Voice.Encryption;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class VoiceCommands(Dictionary<ulong, SemaphoreSlim> joinSemaphores) : ApplicationCommandModule<SlashCommandContext>
{
    private async Task<VoiceClient> JoinAsync(IVoiceGuildChannel? channel, VoiceEncryption? encryption, Func<DisconnectEventArgs, ValueTask>? disconnectHandler = null)
    {
        var guild = Context.Guild!;
        ulong channelId;
        if (channel is not null)
            channelId = channel.Id;
        else if (guild.VoiceStates.TryGetValue(Context.User.Id, out var state))
            channelId = state.ChannelId.GetValueOrDefault();
        else
            throw new("You are not in a voice channel!");

        var client = Context.Client;

        var guildId = guild.Id;
        SemaphoreSlim? semaphore;
        lock (joinSemaphores)
        {
            if (!joinSemaphores.TryGetValue(guildId, out semaphore))
                joinSemaphores[guildId] = semaphore = new SemaphoreSlim(1, 1);
        }
        VoiceClient voiceClient;
        await semaphore.WaitAsync();
        try
        {
            var encryptionProvider = encryption.HasValue ? new StaticVoiceEncryptionProvider(encryption.GetValueOrDefault() switch
            {
                VoiceEncryption.Aes256GcmRtpSize => new Aes256GcmRtpSizeEncryption(),
                VoiceEncryption.XChaCha20Poly1305RtpSize => new XChaCha20Poly1305RtpSizeEncryption(),
                _ => throw new InvalidEnumArgumentException(nameof(encryption), (int)encryption, typeof(VoiceEncryption)),
            }) : null;

            voiceClient = await client.JoinVoiceChannelAsync(guild.Id, channelId, new()
            {
                EncryptionProvider = encryptionProvider,
                ReceiveHandler = new VoiceReceiveHandler(),
                Logger = new ConsoleLogger(LogLevel.Debug),
                //CacheProvider = ConcurrentVoiceClientCacheProvider.Empty,
            });
        }
        finally
        {
            semaphore.Release();
        }
        voiceClient.Disconnect += disconnectHandler;

        await voiceClient.StartAsync();

        await voiceClient.EnterSpeakingStateAsync(new(SpeakingFlags.Microphone));

        return voiceClient;
    }

    [SlashCommand("play", "Plays music")]
    public async Task PlayAsync(IVoiceGuildChannel? channel = null, bool loop = true, VoiceEncryption? encryption = null)
    {
        using CancellationTokenSource cancellationTokenSource = new();

        var voiceClient = await JoinAsync(channel, encryption, args =>
        {
            if (!args.Reconnect)
                cancellationTokenSource.Cancel();

            return default;
        });

        var frameDuration = 2.5f;

        using var outputStream = voiceClient.CreateOutputStream(frameDuration);
        using OpusEncodeStream opusEncodeStream = new(outputStream, PcmFormat.Float, VoiceChannels.Stereo, OpusApplication.Audio, frameDuration);
        //using OpusDecodeStream opusDecodeStream = new(opusEncodeStream, PcmFormat.Short, VoiceChannels.Stereo);
        //using OpusEncodeStream opusEncodeStream2 = new(opusDecodeStream, PcmFormat.Float, VoiceChannels.Stereo, OpusApplication.Audio);

        var url = "https://www.mfiles.co.uk/mp3-downloads/beethoven-symphony6-1.mp3"; // 00:12:08
        //var url = "https://file-examples.com/storage/feee5c69f0643c59da6bf13/2017/11/file_example_MP3_700KB.mp3"; // 00:00:27
        await RespondAsync(InteractionCallback.Message($"Playing: {Path.GetFileNameWithoutExtension(url)}"));

        var token = cancellationTokenSource.Token;
        do
        {
            using var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{url}\" -ac 2 -f f32le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
            })!;

            try
            {
                //ffmpeg.StandardOutput.BaseStream.CopyTo(opusEncodeStream);
                await ffmpeg.StandardOutput.BaseStream.CopyToAsync(opusEncodeStream, token);
                await opusEncodeStream.FlushAsync(token);
            }
            catch (OperationCanceledException)
            {
                ffmpeg.Kill();
                return;
            }
        }
        while (loop);

        await Task.Delay(-1, token);
    }

    [SlashCommand("echo", "Echo!")]
    public async Task EchoAsync(IVoiceGuildChannel? channel = null, VoiceEncryption? encryption = null)
    {
        TaskCompletionSource taskCompletionSource = new();

        var voiceClient = await JoinAsync(channel, encryption, args =>
        {
            if (!args.Reconnect)
                taskCompletionSource.TrySetResult();

            return default;
        });

        var frameDuration = 2.5f;

        using var outputStream = voiceClient.CreateOutputStream(frameDuration, normalizeSpeed: false);
        using OpusEncodeStream opusEncodeStream = new(outputStream, PcmFormat.Float, VoiceChannels.Stereo, OpusApplication.Audio, frameDuration);
        using OpusDecodeStream opusDecodeStream = new(opusEncodeStream, PcmFormat.Float, VoiceChannels.Stereo);
        await RespondAsync(InteractionCallback.Message("Echo!"));

        voiceClient.VoiceReceive += args =>
        {
            opusDecodeStream.Write(args.Frame);
            return default;

            //var frame = args.Frame;
            //var size = frame.Length;
            //var buffer = ArrayPool<byte>.Shared.Rent(size);
            //frame.CopyTo(buffer);
            //return outputStream.WriteAsync(buffer.AsMemory(0, size));
        };

        await taskCompletionSource.Task;
    }

    [SlashCommand("record", "Record!")]
    public async Task RecordAsync(IVoiceGuildChannel? channel = null, VoiceEncryption? encryption = null)
    {
        TaskCompletionSource taskCompletionSource = new();

        using var ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-f f32le -ar 48000 -ac 2 -i pipe:0 recording-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.wav",
            RedirectStandardInput = true,
        })!;

        var voiceClient = await JoinAsync(channel, encryption, args =>
        {
            if (!args.Reconnect)
                taskCompletionSource.TrySetResult();

            return default;
        });
        await RespondAsync(InteractionCallback.Message("Recording!"));

        using OpusDecodeStream opusDecodeStream = new(ffmpeg.StandardInput.BaseStream, PcmFormat.Float, VoiceChannels.Stereo);

        voiceClient.VoiceReceive += args =>
        {
            opusDecodeStream.Write(args.Frame);
            return default;
        };

        await taskCompletionSource.Task;
        ffmpeg.Kill();
    }

    public enum VoiceEncryption : byte
    {
        Aes256GcmRtpSize,
        XChaCha20Poly1305RtpSize,
    }

    public class StaticVoiceEncryptionProvider(IVoiceEncryption encryption) : IVoiceEncryptionProvider
    {
        public IVoiceEncryption GetEncryption(IReadOnlyList<string> modes)
        {
            if (modes.Contains(encryption.Name))
                return encryption;

            throw new InvalidOperationException($"Encryption mode '{encryption.Name}' is not supported.");
        }
    }
}
