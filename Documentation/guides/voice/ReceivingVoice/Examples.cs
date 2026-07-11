using System.Threading.Channels;

using NetCord.Gateway.Voice;

namespace MyBot;

internal static class Examples
{
    public static void VoiceReceiveEvent(VoiceClient voiceClient)
    {
        voiceClient.VoiceReceive += args =>
        {
            if (voiceClient.Cache.SsrcUsers.TryGetValue(args.Ssrc, out var userId))
                Console.WriteLine($"Received audio from user {userId}");
            else
                Console.WriteLine($"Received audio from unknown user with SSRC {args.Ssrc}");

            return default;
        };
    }

    public static void VoiceReceiveEventAsyncHandler(VoiceClient voiceClient)
    {
        voiceClient.VoiceReceive += args =>
        {
            // You may consider renting a buffer
            // from 'ArrayPool<T>' in a real application
            var ownedFrame = args.Frame.ToArray();

            return HandleAsync(ownedFrame, args.Ssrc);

            static async ValueTask HandleAsync(byte[] frame, uint ssrc)
            {
                await Task.Delay(100); // Simulate async processing

                Console.WriteLine($"Processed audio frame from SSRC {ssrc}");
            }
        };
    }

    public static async Task BufferingReceivedAudio(VoiceClient voiceClient)
    {
        // Configure the channel to drop frames when the buffer is full;
        // 1000 frames means from 2.5 to 120 seconds of
        // audio depending on the Opus frame duration;
        // Dropping frames should generally never happen, but it is a good
        // idea to handle it just in case to avoid running out of memory
        var channel = Channel.CreateBounded<(byte[] Frame, uint? Timestamp)>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
        });

        var writer = channel.Writer;

        voiceClient.VoiceReceive += args =>
        {
            // You may consider renting a buffer from 'ArrayPool<T>' in a real application
            var frame = args.Frame.ToArray();

            return writer.WriteAsync((frame, args.Timestamp));
        };

        await foreach (var (frame, timestamp) in channel.Reader.ReadAllAsync())
            Console.WriteLine($"Received audio frame of size {frame.Length} with timestamp {timestamp}");
    }

    public static void CreateOpusDecodeStream(Stream stream)
    {
        OpusDecodeStream opusDecodeStream = new(stream,
                                                PcmFormat.Float,
                                                VoiceChannels.Stereo);
    }
}
