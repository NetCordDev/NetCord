using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;

using NetCord.Gateway.Voice;
using NetCord.Gateway.Voice.Encryption;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class VoiceCommands(Dictionary<ulong, SemaphoreSlim> joinSemaphores) : ApplicationCommandModule<SlashCommandContext>
{
    private async Task<VoiceClient> JoinAsync(VoiceEncryption? encryption, Func<bool, ValueTask>? disconnectHandler = null)
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

        await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        return voiceClient;
    }

    [SlashCommand("play", "Plays music")]
    public async Task PlayAsync(VoiceEncryption? encryption = null)
    {
        using CancellationTokenSource cancellationTokenSource = new();

        var voiceClient = await JoinAsync(encryption, reconnect =>
        {
            if (!reconnect)
                cancellationTokenSource.Cancel();

            return default;
        });

        ////VoiceClient voiceClient2 = new(voiceClient.UserId, voiceClient.SessionId, voiceClient.Endpoint, voiceClient.GuildId, voiceClient.Token);

        ////await voiceClient.ResumeAsync(voiceClient.SequenceNumber);

        //await voiceClient.StartAsync();

        //await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        //await voiceClient.ResumeAsync(voiceClient.SequenceNumber);

        //await voiceClient.StartAsync();

        //await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        //await voiceClient.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.Empty);

        //await voiceClient.StartAsync();

        //await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        //await voiceClient.ResumeAsync(voiceClient.SequenceNumber);

        //voiceClient.Abort();
        //await voiceClient.StartAsync();

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

        //_ = Task.Run(async () =>
        //{
        //    while (true)
        //    {
        //        voiceClient.Abort();
        //        await voiceClient.StartAsync();
        //        await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);
        //        outputStream = voiceClient.CreateOutputStream();
        //        opusEncodeStream = new(outputStream, PcmFormat.Float, VoiceChannels.Stereo, OpusApplication.Audio);
        //        //await voiceClient.ResumeAsync(voiceClient.SequenceNumber);
        //        await Task.Delay(1000);
        //    }
        //});

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

        var voiceClient = await JoinAsync(encryption, reconnect =>
        {
            if (!reconnect)
                taskCompletionSource.TrySetResult();

            return default;
        });


        //await voiceClient.ResumeAsync(voiceClient.SequenceNumber);

        //voiceClient = new(voiceClient.UserId, voiceClient.SessionId, voiceClient.Endpoint, voiceClient.GuildId, voiceClient.Token, new() { ReceiveHandler = new VoiceReceiveHandler(), Logger = new ConsoleLogger(LogLevel.Debug) });

        //await voiceClient.CloseAsync();

        //await voiceClient.StartAsync();

        //await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        using var outputStream = voiceClient.CreateOutputStream(false);
        await RespondAsync(InteractionCallback.Message("Echo!"));

        voiceClient.VoiceReceive += args =>
        {
            var frame = args.Frame;
            var size = frame.Length;
            var buffer = ArrayPool<byte>.Shared.Rent(size);
            frame.CopyTo(buffer);
            return outputStream.WriteAsync(buffer.AsMemory(0, size));

            //outputStream.Write(args.Frame);
            //return default;

            //return outputStream.WriteAsync(args.Frame, default);

            //if (!Unsafe.IsNullRef(ref MemoryMarshal.GetReference(args.Frame.Span)))
            //await outputStream.WriteAsync(args.Frame).ConfigureAwait(false);
        };

        //ushort? lastSequenceNumber = null;

        //voiceClient.VoiceReceive += async args =>
        //{
        //    ushort framesMissed;

        //    //if (lastSequenceNumber.HasValue)
        //    //{
        //    //    var context = args.GetContext(lastSequenceNumber.GetValueOrDefault());
        //    //    if (!context.InOrder)
        //    //        return;

        //    //    framesMissed = context.FramesMissed;
        //    //}
        //    //else
        //    //    framesMissed = 0;

        //    //lastSequenceNumber = args.SequenceNumber;

        //    var payloadSize = args.PayloadSize;

        //    //var array = ArrayPool<byte>.Shared.Rent(payloadSize);

        //    var array = new byte[payloadSize];

        //    var result = args.Decrypt(array);

        //    //for (ushort i = 0; i < framesMissed; i++)
        //    //    await outputStream.WriteAsync(null);

        //    await outputStream.WriteAsync(array.AsMemory()[result.FrameIndex..payloadSize]);

        //ArrayPool<byte>.Shared.Return(array);

        //var payloadSize = args.PayloadSize;

        //var array = ArrayPool<byte>.Shared.Rent(payloadSize);

        //var result = args.Decrypt(array, lastSequenceNumber);

        //if (result.Status is not ContinuousVoiceDecryptionStatus.Ok)
        //    return;

        //lastSequenceNumber = args.SequenceNumber;

        //for (var i = 0; i < result.FramesMissed; i++)
        //    await outputStream.WriteAsync(null);

        //await outputStream.WriteAsync(array.AsMemory()[result.FrameIndex..payloadSize]);

        //ArrayPool<byte>.Shared.Return(array);
        //};

        //voiceClient.VoiceReceive += async args =>
        //{
        //    var sequenceNumber = args.SequenceNumber;
        //    if (lastSequenceNumber.HasValue)
        //    {
        //        var diff = (ushort)(sequenceNumber - lastSequenceNumber.GetValueOrDefault());
        //        switch (diff)
        //        {
        //            case 1:
        //                lastSequenceNumber = sequenceNumber;
        //                break;
        //            case 0 or > ushort.MaxValue / 2:
        //                return;
        //            default:
        //                do
        //                    await outputStream.WriteAsync(null);
        //                while (--diff > 1);
        //                break;
        //        }
        //    }
        //    else
        //        lastSequenceNumber = sequenceNumber;

        //    var payloadSize = args.PayloadSize;

        //    var array = ArrayPool<byte>.Shared.Rent(payloadSize);

        //    var result = args.Decrypt(array);

        //    if (lastSequenceNumber.HasValue)
        //    {
        //        for (var i = sequenceNumber - lastSequenceNumber.GetValueOrDefault(); i > 1; i--)
        //            await outputStream.WriteAsync(null);
        //    }

        //    lastSequenceNumber = sequenceNumber;

        //    await outputStream.WriteAsync(array.AsMemory()[result.FrameIndex..payloadSize]);

        //    ArrayPool<byte>.Shared.Return(array);
        //};

        //voiceClient.VoiceReceive += args => outputStream.WriteAsync(args);

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
