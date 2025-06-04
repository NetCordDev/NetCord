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
    private async Task<VoiceClient> JoinAsync(VoiceEncryption? encryption, Func<DisconnectEventArgs, ValueTask>? disconnectHandler = null)
    {
        var guild = Context.Guild!;
        if (!guild.VoiceStates.TryGetValue(Context.User.Id, out var state))
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
                VoiceEncryption.XSalsa20Poly1305 => new XSalsa20Poly1305Encryption(),
                VoiceEncryption.XSalsa20Poly1305Lite => new XSalsa20Poly1305LiteEncryption(),
                VoiceEncryption.XSalsa20Poly1305LiteRtpSize => new XSalsa20Poly1305LiteRtpSizeEncryption(),
                VoiceEncryption.XSalsa20Poly1305Suffix => new XSalsa20Poly1305SuffixEncryption(),
                VoiceEncryption.Aes256Gcm => new Aes256GcmEncryption(),
                VoiceEncryption.Aes256GcmRtpSize => new Aes256GcmRtpSizeEncryption(),
                VoiceEncryption.XChaCha20Poly1305RtpSize => new XChaCha20Poly1305RtpSizeEncryption(),
                _ => throw new InvalidEnumArgumentException(nameof(encryption), (int)encryption, typeof(VoiceEncryption)),
            }) : null;

            voiceClient = await client.JoinVoiceChannelAsync(guild.Id, state.ChannelId.GetValueOrDefault(), new()
            {
                EncryptionProvider = encryptionProvider,
                ReceiveHandler = new VoiceReceiveHandler(),
                Logger = new ConsoleLogger(LogLevel.Debug),
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
    public async Task PlayAsync(VoiceEncryption? encryption = null)
    {
        using CancellationTokenSource cancellationTokenSource = new();

        var voiceClient = await JoinAsync(encryption, args =>
        {
            if (!args.Reconnect)
                cancellationTokenSource.Cancel();

            return default;
        });

        var outputStream = voiceClient.CreateOutputStream();
        OpusEncodeStream opusEncodeStream = new(outputStream, PcmFormat.Float, VoiceChannels.Stereo, OpusApplication.Audio);

        var url = "https://www.mfiles.co.uk/mp3-downloads/beethoven-symphony6-1.mp3"; // 00:12:08
        //var url = "https://file-examples.com/storage/feee5c69f0643c59da6bf13/2017/11/file_example_MP3_700KB.mp3"; // 00:00:27
        await RespondAsync(InteractionCallback.Message($"Playing: {Path.GetFileNameWithoutExtension(url)}"));
        using var ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{url}\" -ac 2 -f f32le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
        })!;

        var token = cancellationTokenSource.Token;
        try
        {
            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(opusEncodeStream, token);
            await opusEncodeStream.FlushAsync(token);
            await Task.Delay(-1, token);
        }
        catch (OperationCanceledException)
        {
            ffmpeg.Kill();
        }
    }

    [SlashCommand("echo", "Echo!")]
    public async Task EchoAsync(VoiceEncryption? encryption = null)
    {
        TaskCompletionSource taskCompletionSource = new();

        var voiceClient = await JoinAsync(encryption, args =>
        {
            if (!args.Reconnect)
                taskCompletionSource.TrySetResult();

            return default;
        });

        using var outputStream = voiceClient.CreateOutputStream(false);
        await RespondAsync(InteractionCallback.Message("Echo!"));

        voiceClient.VoiceReceive += args =>
        {
            outputStream.Write(args.Frame);
            return default;

            //var frame = args.Frame;
            //var size = frame.Length;
            //var buffer = ArrayPool<byte>.Shared.Rent(size);
            //frame.CopyTo(buffer);
            //return outputStream.WriteAsync(buffer.AsMemory(0, size));
        };

        await taskCompletionSource.Task;
    }

    public enum VoiceEncryption : byte
    {
        XSalsa20Poly1305,
        XSalsa20Poly1305Lite,
        XSalsa20Poly1305LiteRtpSize,
        XSalsa20Poly1305Suffix,
        Aes256Gcm,
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
