using System.Diagnostics;

using NetCord;
using NetCord.Gateway.Voice;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class VoiceModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("play", "Plays music", Contexts = [InteractionContextType.Guild])]
    public async Task PlayAsync(string track)
    {
        // Check if the specified track is a well formed uri
        if (!Uri.IsWellFormedUriString(track, UriKind.Absolute))
        {
            await RespondAsync(InteractionCallback.Message("Invalid track!"));
            return;
        }

        var guild = Context.Guild!;

        // Get the user voice state
        if (!guild.VoiceStates.TryGetValue(Context.User.Id, out var voiceState))
        {
            await RespondAsync(InteractionCallback.Message("You are not connected to any voice channel!"));
            return;
        }

        var client = Context.Client;

        // You should check if the bot is already connected to the voice channel.
        // If so, you should use an existing 'VoiceClient' instance instead of creating a new one.
        // You also need to add a synchronization here. 'JoinVoiceChannelAsync' should not be used concurrently for the same guild
        var voiceClient = await client.JoinVoiceChannelAsync(
            guild.Id,
            voiceState.ChannelId.GetValueOrDefault());

        // Connect
        await voiceClient.StartAsync();

        // Enter speaking state, to be able to send voice
        await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        // Respond to the interaction
        await RespondAsync(InteractionCallback.Message($"Playing {Path.GetFileName(track)}!"));

        // Create a stream that sends voice to Discord
        var outStream = voiceClient.CreateOutputStream();

        // We create this stream to automatically convert the PCM data returned by FFmpeg to Opus data.
        // The Opus data is then written to 'outStream' that sends the data to Discord
        OpusEncodeStream stream = new(outStream, PcmFormat.Short, VoiceChannels.Stereo, OpusApplication.Audio);

        ProcessStartInfo startInfo = new("ffmpeg")
        {
            RedirectStandardOutput = true,
        };
        var arguments = startInfo.ArgumentList;

        // Set reconnect attempts in case of a lost connection to 1
        arguments.Add("-reconnect");
        arguments.Add("1");

        // Set reconnect attempts in case of a lost connection for streamed media to 1
        arguments.Add("-reconnect_streamed");
        arguments.Add("1");

        // Set the maximum delay between reconnection attempts to 5 seconds
        arguments.Add("-reconnect_delay_max");
        arguments.Add("5");

        // Specify the input
        arguments.Add("-i");
        arguments.Add(track);

        // Set the logging level to quiet mode
        arguments.Add("-loglevel");
        arguments.Add("-8");

        // Set the number of audio channels to 2 (stereo)
        arguments.Add("-ac");
        arguments.Add("2");

        // Set the output format to 16-bit signed little-endian
        arguments.Add("-f");
        arguments.Add("s16le");

        // Set the audio sampling rate to 48 kHz
        arguments.Add("-ar");
        arguments.Add("48000");

        // Direct the output to stdout
        arguments.Add("pipe:1");

        // Start the FFmpeg process
        var ffmpeg = Process.Start(startInfo)!;

        // Copy the FFmpeg stdout to 'stream', which encodes the voice using Opus and passes it to 'outStream'
        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);

        // Flush 'stream' to make sure all the data has been sent and to indicate to Discord that we have finished sending
        await stream.FlushAsync();
    }

    [SlashCommand("echo", "Creates echo", Contexts = [InteractionContextType.Guild])]
    public async Task<string> EchoAsync()
    {
        var guild = Context.Guild!;
        var userId = Context.User.Id;

        // Get the user voice state
        if (!guild.VoiceStates.TryGetValue(userId, out var voiceState))
            return "You are not connected to any voice channel!";

        var client = Context.Client;

        // You should check if the bot is already connected to the voice channel.
        // If so, you should use an existing 'VoiceClient' instance instead of creating a new one.
        // You also need to add a synchronization here. 'JoinVoiceChannelAsync' should not be used concurrently for the same guild
        var voiceClient = await client.JoinVoiceChannelAsync(
            guild.Id,
            voiceState.ChannelId.GetValueOrDefault(),
            new() { RedirectInputStreams = true /* Required to receive voice */ });

        // Connect
        await voiceClient.StartAsync();

        // Enter speaking state, to be able to send voice
        await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        // Create a stream that sends voice to Discord
        var outStream = voiceClient.CreateOutputStream(normalizeSpeed: false);

        voiceClient.VoiceReceive += args =>
        {
            // Pass current user voice directly to the output to create echo
            if (args.UserId == userId)
                return outStream.WriteAsync(args.Frame);
            return default;
        };

        // Return the response
        return "Echo!";
    }
}
