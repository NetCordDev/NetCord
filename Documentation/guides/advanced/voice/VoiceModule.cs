using System.Diagnostics;

using NetCord;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class VoiceModule : ApplicationCommandModule<SlashCommandContext>
{
    private static async Task<VoiceClient> JoinAsync(GatewayClient client, ulong guildId, ulong channelId)
    {
        TaskCompletionSource<VoiceState> stateTaskCompletionSource = new();
        TaskCompletionSource<VoiceServerUpdateEventArgs> serverTaskCompletionSource = new();

        // Subscribe to the events to receive the data needed to create a 'VoiceClient' instance
        client.VoiceStateUpdate += HandleVoiceStateUpdateAsync;
        client.VoiceServerUpdate += HandleVoiceServerUpdateAsync;

        // Join a channel
        await client.UpdateVoiceStateAsync(new VoiceStateProperties(guildId, channelId));

        var timeout = TimeSpan.FromSeconds(1);
        VoiceState state;
        VoiceServerUpdateEventArgs server;
        try
        {
            // Wait for the events with 1 second timeout
            state = await stateTaskCompletionSource.Task.WaitAsync(timeout);
            server = await serverTaskCompletionSource.Task.WaitAsync(timeout);
        }
        catch (TimeoutException)
        {
            // Return an error on timeout
            throw new($"Failed to join <#{channelId}>!");
        }

        // Create a 'VoiceClient' instance with the data from the events
        VoiceClient voiceClient = new(state.UserId, state.SessionId, server.Endpoint!, server.GuildId, server.Token, new VoiceClientConfiguration()
        {
            RedirectInputStreams = true,
        });

        // Connect
        await voiceClient.StartAsync();

        // Wait for ready
        await voiceClient.ReadyAsync;

        // Enter speaking state, to be able to send voice
        await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        return voiceClient;

        ValueTask HandleVoiceStateUpdateAsync(VoiceState arg)
        {
            // Filter received events
            if (arg.UserId == client.User!.Id && arg.GuildId == guildId)
            {
                // Unsubscribe from the event
                client.VoiceStateUpdate -= HandleVoiceStateUpdateAsync;

                // Pass the event data to 'stateTaskCompletionSource'
                stateTaskCompletionSource.SetResult(arg);
            }
            return default;
        }

        ValueTask HandleVoiceServerUpdateAsync(VoiceServerUpdateEventArgs arg)
        {
            // Filter received events
            if (arg.GuildId == guildId)
            {
                // Unsubscribe from the event
                client.VoiceServerUpdate -= HandleVoiceServerUpdateAsync;

                // Pass the event data to 'serverTaskCompletionSource'
                serverTaskCompletionSource.SetResult(arg);
            }
            return default;
        }
    }

    [SlashCommand("play", "Plays music", DMPermission = false)]
    public async Task PlayAsync(string track)
    {
        // Check if the specified track is a well formed uri
        if (!Uri.IsWellFormedUriString(track, UriKind.Absolute))
            throw new("Invalid track!");

        // Get the user voice state
        if (!Context.Guild!.VoiceStates.TryGetValue(Context.User.Id, out var voiceState))
            throw new("You are not connected to any voice channel!");

        // You should check if the bot is already connected to the voice channel.
        // If so, you should use an existing 'VoiceClient' instance instead of creating a new one
        var voiceClient = await JoinAsync(Context.Client, Context.Guild.Id, voiceState.ChannelId.GetValueOrDefault());

        // Respond to the interaction
        await RespondAsync(InteractionCallback.ChannelMessageWithSource($"Playing {Path.GetFileName(track)}!"));

        // Create a stream that sends voice to Discord
        var outStream = voiceClient.CreateOutputStream();

        // We create this stream to automatically convert the PCM data returned by FFmpeg to Opus data.
        // The Opus data is then written to 'outStream' that sends the data to Discord
        OpusEncodeStream stream = new(outStream, VoiceChannels.Stereo, OpusApplication.Audio);

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

    [SlashCommand("echo", "Creates echo", DMPermission = false)]
    public async Task EchoAsync()
    {
        var userId = Context.User.Id;

        // Get the user voice state
        if (!Context.Guild!.VoiceStates.TryGetValue(userId, out var voiceState))
            throw new("You are not connected to any voice channel!");

        // You should check if the bot is already connected to the voice channel.
        // If so, you should use an existing 'VoiceClient' instance instead of creating a new one
        var voiceClient = await JoinAsync(Context.Client, Context.Guild.Id, voiceState.ChannelId.GetValueOrDefault());

        // Create a stream that sends voice to Discord
        var outStream = voiceClient.CreateOutputStream(normalizeSpeed: false);

        voiceClient.VoiceReceive += args =>
        {
            // Pass current user voice directly to the output to create echo
            if (args.UserId == 803230269111926786)
                return outStream.WriteAsync(args.Frame);
            return default;
        };

        // Respond to the interaction
        await RespondAsync(InteractionCallback.ChannelMessageWithSource("Echo!"));
    }
}
