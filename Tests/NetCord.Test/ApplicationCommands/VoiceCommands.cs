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
                //ReceiveHandler = new RawVoiceReceiveHandler(),
                ReceiveHandler = new BufferedVoiceReceiveHandler(new()
                {
                    BufferDuration = 40,
                    StartupDuration = 20,
                    ResynchronizationDuration = 1000,
                }),
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
    public async Task PlayAsync(IVoiceGuildChannel? channel = null,
                                bool loop = true,
                                VoiceEncryption? encryption = null,
                                PcmFormat pcmFormat = PcmFormat.Float,
                                VoiceChannels voiceChannels = VoiceChannels.Stereo,
                                OpusApplication opusApplication = OpusApplication.Audio,
                                float frameDuration = 2.5f)
    {
        using CancellationTokenSource cancellationTokenSource = new();

        var voiceClient = await JoinAsync(channel, encryption, args =>
        {
            if (!args.Reconnect)
                cancellationTokenSource.Cancel();

            return default;
        });

        using var outputStream = voiceClient.CreateVoiceStream(new() { FrameDuration = frameDuration });
        using OpusEncodeStream opusEncodeStream = new(outputStream, pcmFormat, voiceChannels, opusApplication, new() { FrameDuration = frameDuration });
        //using OpusDecodeStream opusDecodeStream = new(opusEncodeStream, pcmFormat, voiceChannels);
        //using OpusEncodeStream opusEncodeStream2 = new(opusDecodeStream, pcmFormat, voiceChannels, opusApplication);

        var url = "https://www.mfiles.co.uk/mp3-downloads/beethoven-symphony6-1.mp3"; // 00:12:08
        //var url = @"C:\Users\Kuba\Downloads\jackhammer.wav";
        //var url = "https://file-examples.com/storage/feee5c69f0643c59da6bf13/2017/11/file_example_MP3_700KB.mp3"; // 00:00:27
        await RespondAsync(InteractionCallback.Message($"Playing: {Path.GetFileNameWithoutExtension(url)}"));

        var token = cancellationTokenSource.Token;
        do
        {
            using var ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{url}\" -ac {(byte)voiceChannels} -f {(pcmFormat is PcmFormat.Short ? "s16le" : "f32le")} -ar 48000 pipe:1",
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

            await Task.Delay(2000);
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

        //using var outputStream = voiceClient.CreateVoiceStream(frameDuration, normalizeSpeed: false);
        //using OpusEncodeStream opusEncodeStream = new(outputStream, PcmFormat.Float, VoiceChannels.Stereo, OpusApplication.Audio, frameDuration);
        //using OpusDecodeStream opusDecodeStream = new(opusEncodeStream, PcmFormat.Float, VoiceChannels.Stereo);
        await RespondAsync(InteractionCallback.Message("Echo!"));

        voiceClient.VoiceReceive += args =>
        {
            if (args.Timestamp is { } timestamp)
                voiceClient.SendVoice(args.SequenceNumber, timestamp, args.Frame);
            else
                Console.WriteLine($"Frame {args.SequenceNumber} got lost");

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
    public async Task RecordAsync(IVoiceGuildChannel? channel = null,
                                  VoiceEncryption? encryption = null,
                                  PcmFormat pcmFormat = PcmFormat.Float,
                                  VoiceChannels voiceChannels = VoiceChannels.Stereo)
    {
        TaskCompletionSource taskCompletionSource = new();

        using var ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-f {(pcmFormat is PcmFormat.Short ? "s16le" : "f32le")} -ar 48000 -ac {(byte)voiceChannels} -i pipe:0 recording-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.wav",
            RedirectStandardInput = true,
        })!;

        using var ffmpeg2 = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-f {(pcmFormat is PcmFormat.Short ? "s16le" : "f32le")} -ar 48000 -ac {(byte)voiceChannels} -i pipe:0 recording-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}-2.wav",
            RedirectStandardInput = true,
        })!;

        var voiceClient = await JoinAsync(channel, encryption, args =>
        {
            if (!args.Reconnect)
                taskCompletionSource.TrySetResult();

            return default;
        });
        await RespondAsync(InteractionCallback.Message("Recording!"));

        // using OpusDecodeStream opusDecodeStream = new(ffmpeg.StandardInput.BaseStream, pcmFormat, voiceChannels);

        using OpusDecoder decoder = new(voiceChannels);
        using OpusDecoder decoder2 = new(voiceChannels);
        var bufferSize = Opus.GetFrameBufferSize(Opus.GetSamplesPerChannel(Opus.MaxFrameDuration), pcmFormat, voiceChannels);
        var buffer = new byte[bufferSize];

        voiceClient.VoiceReceive += args =>
        {
            // Console.WriteLine(args.IsLost);
            if (args.IsLost)
                Console.WriteLine($"Lost frame {args.SequenceNumber} with {args.AsLost().SamplesPerChannel} samples");

            if (!args.IsLost)
            {
                voiceClient.SendVoice(args.SequenceNumber, args.Timestamp, args.Frame);
            }

            {
                var frameSize = args.IsLost ? args.AsLost().SamplesPerChannel : Opus.GetSamplesPerChannel(Opus.MaxFrameDuration);
                var samples = decoder.DecodeFloat(args.Frame, buffer.AsSpan(0, Opus.GetFrameBufferSize(frameSize, PcmFormat.Float, VoiceChannels.Stereo)), frameSize, false);
                ffmpeg.StandardInput.BaseStream.Write(buffer, 0, Opus.GetFrameBufferSize(samples, pcmFormat, voiceChannels));
            }

            {
                var frameSize = args.IsLost ? args.AsLost().SamplesPerChannel : Opus.GetSamplesPerChannel(Opus.MaxFrameDuration);
                var samples = decoder2.DecodeFloat(args.IsLost ? args.AsLost().FecData : args.Frame, buffer.AsSpan(0, Opus.GetFrameBufferSize(frameSize, PcmFormat.Float, VoiceChannels.Stereo)), frameSize, args.IsLost && args.AsLost().DecodeFec);
                ffmpeg2.StandardInput.BaseStream.Write(buffer, 0, Opus.GetFrameBufferSize(samples, pcmFormat, voiceChannels));
            }

            // if (args.IsLost)
            // {
            //     var lostArgs = args.AsLost();
            // }

            // if (args.Timestamp.HasValue)
            //     voiceClient.SendVoice(args.SequenceNumber, args.Timestamp.GetValueOrDefault(), args.Frame);

            // // if (args.CanCorrectLoss)
            // //     Console.WriteLine($"Frame {args.SequenceNumber} got lost but can be corrected");
            // // else if (!args.Timestamp.HasValue)
            // //     Console.WriteLine($"Frame {args.SequenceNumber} got lost and cannot be corrected");

            // Console.WriteLine($"Received frame {args.SequenceNumber} (canCorrectLoss: {args.CanCorrectLoss}, timestamp: {(args.Timestamp.HasValue ? args.Timestamp.GetValueOrDefault().ToString() : "null")}): {string.Join(' ', args.Frame.ToArray().Take(10))}...");

            // {
            //     var samples = decoder.DecodeFloat(args.Timestamp.HasValue ? args.Frame : null, buffer, Opus.GetSamplesPerChannel(20), false);
            //     ffmpeg.StandardInput.BaseStream.Write(buffer, 0, Opus.GetFrameBufferSize(samples, pcmFormat, voiceChannels));
            // }

            // {
            //     var samples = decoder2.DecodeFloat(args.Frame, buffer, Opus.GetSamplesPerChannel(20), args.CanCorrectLoss);
            //     ffmpeg2.StandardInput.BaseStream.Write(buffer, 0, Opus.GetFrameBufferSize(samples, pcmFormat, voiceChannels));
            // }

            // if (args.CanCorrectLoss)
            // {
            //     var samples = decoder.DecodeFloat(args.Frame, buffer, Opus.GetSamplesPerChannel(Opus.MaxFrameDuration), true);
            //     ffmpeg2.StandardInput.BaseStream.Write(buffer, 0, Opus.GetFrameBufferSize(samples, pcmFormat, voiceChannels));
            // }
            // else
            // {
            //     var samples = decoder.DecodeFloat(args.Frame, buffer, Opus.GetSamplesPerChannel(Opus.MaxFrameDuration), false);
            //     ffmpeg2.StandardInput.BaseStream.Write(buffer, 0, Opus.GetFrameBufferSize(samples, pcmFormat, voiceChannels));
            // }

            return default;

            // if (!args.Timestamp.HasValue)
            // {
            //     unsafe
            //     {
            //         fixed (byte* frame = args.Frame)
            //             Console.WriteLine($"Frame {args.SequenceNumber} got lost {(nint)frame}");
            //     }
            // }

            // opusDecodeStream.Write(args.Frame);
            // return default;
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
