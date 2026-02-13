using System.Diagnostics;

using NetCord.Gateway.Voice;

namespace MyBot;

internal static class Examples
{
    public static void CreateVoiceStream(VoiceClient voiceClient)
    {
        var voiceStream = voiceClient.CreateVoiceStream();
    }

    public static void CreateVoiceStreamWithConfiguration(VoiceClient voiceClient)
    {
        var voiceStream = voiceClient.CreateVoiceStream(new VoiceStreamConfiguration
        {
            NormalizeSpeed = false,
            FrameDuration = 60,
        });
    }

    public static void CreateOpusEncodeStream(Stream voiceStream)
    {
        OpusEncodeStream opusEncodeStream = new(voiceStream,
                                                PcmFormat.Float,
                                                VoiceChannels.Stereo,
                                                OpusApplication.Audio);
    }

    public static void CreateOpusEncodeStreamWithConfiguration(Stream voiceStream)
    {
        OpusEncodeStream opusEncodeStream = new(voiceStream,
                                                PcmFormat.Float,
                                                VoiceChannels.Stereo,
                                                OpusApplication.Audio,
                                                new OpusEncodeStreamConfiguration
                                                {
                                                    Segment = false,
                                                    FrameDuration = 2.5f,
                                                });
    }

    public static async Task FfmpegAsync(OpusEncodeStream opusEncodeStream, string input)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            ArgumentList =
            {
                // Input file
                "-i", input,
                // Output format 32-bit float PCM with native endianness
                "-f", BitConverter.IsLittleEndian ? "f32le" : "f32be",
                // Sampling rate 48kHz
                "-ar", "48000",
                // 2 channels (stereo)
                "-ac", "2",
                // Output to stdout
                "pipe:1",
            },
            RedirectStandardOutput = true,
        })!;

        var ffmpegOutput = process.StandardOutput.BaseStream;

        // Copy the FFmpeg output directly to the OpusEncodeStream
        await ffmpegOutput.CopyToAsync(opusEncodeStream);

        // Flush the OpusEncodeStream to ensure all data is sent,
        // appended by the silence frames to prevent audio interpolation
        await opusEncodeStream.FlushAsync();
    }
}
