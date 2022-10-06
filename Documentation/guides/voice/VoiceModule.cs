using NetCord.Gateway.Voice;
using NetCord.Gateway;
using NetCord;

using NetCord.Services.ApplicationCommands;
using System.Diagnostics;

namespace MyBot;

public class VoiceModule : ApplicationCommandModule<SlashCommandContext>
{
    private static async Task<VoiceClient> JoinAsync(GatewayClient client, Snowflake guildId, Snowflake channelId)
    {
        TaskCompletionSource<VoiceState> stateTaskCompletionSource = new();
        TaskCompletionSource<VoiceServerUpdateEventArgs> serverTaskCompletionSource = new();

        client.VoiceStateUpdate += Client_VoiceStateUpdate;
        client.VoiceServerUpdate += Client_VoiceServerUpdate;

        await client.UpdateVoiceStateAsync(new VoiceStateProperties(guildId, channelId)); // Joining a channel

        var timeSpan = TimeSpan.FromSeconds(1);
        VoiceState state;
        VoiceServerUpdateEventArgs server;
        try
        {
            state = await stateTaskCompletionSource.Task.WaitAsync(timeSpan);
            server = await serverTaskCompletionSource.Task.WaitAsync(timeSpan);
        }
        catch (TimeoutException)
        {
            throw new($"Failed to join <#{channelId}>!");
        }

        VoiceClient voiceClient = new(state.UserId, state.SessionId, server.Endpoint!, server.GuildId, server.Token); // Creating VoiceClient instance with data from events
        await voiceClient.StartAsync(); // Connecting
        await voiceClient.ReadyAsync;
        await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone); // Entering speaking state, to be able to send voice
        return voiceClient;

        ValueTask Client_VoiceStateUpdate(VoiceState arg)
        {
            client.VoiceStateUpdate -= Client_VoiceStateUpdate;
            if (arg.UserId == client.User!.Id && arg.GuildId == guildId)
                stateTaskCompletionSource.SetResult(arg);
            return default;
        }

        ValueTask Client_VoiceServerUpdate(VoiceServerUpdateEventArgs arg)
        {
            client.VoiceServerUpdate -= Client_VoiceServerUpdate;
            if (arg.GuildId == guildId)
                serverTaskCompletionSource.SetResult(arg);
            return default;
        }
    }

    [SlashCommand("join", "Joins a voice channel", DMPermission = false)]
    public async Task JoinAsync()
    {
        // You probably need to add a check if bot is already connected to a voice channel in current guild

        if (Context.Guild!.VoiceStates.TryGetValue(Context.User.Id, out var voiceState)) // Getting user voice state
        {
            var channelId = voiceState.ChannelId.GetValueOrDefault();
            var voiceClient = await JoinAsync(Context.Client, Context.Guild.Id, channelId); // You probably need to save the VoiceClient instance somewhere

            await RespondAsync(InteractionCallback.ChannelMessageWithSource($"Joined <#{channelId}>!"));
        }
        else
            throw new("You are not connected to any voice channel!");
    }

    [SlashCommand("play", "Plays music", DMPermission = false)]
    public async Task PlayAsync(string track)
    {
        // This will prevent from FFmpeg injection
        if (!Uri.IsWellFormedUriString(track, UriKind.Absolute))
            throw new("Invalid track!");

        if (Context.Guild!.VoiceStates.TryGetValue(Context.User.Id, out var voiceState)) // Getting user voice state
        {
            // You should check if the bot is already connected to a voice channel in current guild, if yes, you should use existing VoiceClient instance instead of creating a new one

            var voiceClient = await JoinAsync(Context.Client, Context.Guild.Id, voiceState.ChannelId.GetValueOrDefault()); // You probably need to save the VoiceClient instance somewhere

            await RespondAsync(InteractionCallback.ChannelMessageWithSource($"Playing {Path.GetFileName(track)}!"));

            var stream = voiceClient.CreatePCMStream(OpusApplication.Audio); // This stream should be saved somewhere

            // Now, you can write to the stream to send voice to Discord. The data should be PCM data. You can get it using FFmpeg.
            var ffmpeg = Process.Start(new ProcessStartInfo("ffmpeg", $"-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -i \"{track}\" -loglevel -8 -ac 2 -f s16le -ar 48000 pipe:1")
            {
                RedirectStandardOutput = true,
            })!;
            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);
            await stream.FlushAsync();
        }
        else
            throw new("You are not connected to any voice channel!");
    }

    [SlashCommand("echo", "Creates echo", DMPermission = false)]
    public async Task EchoAsync()
    {
        if (Context.Guild!.VoiceStates.TryGetValue(Context.User.Id, out var voiceState))
        {
            var voiceClient = await JoinAsync(Context.Client, Context.Guild.Id, voiceState.ChannelId.GetValueOrDefault());
            var stream = voiceClient.CreateDirectPCMStream(OpusApplication.Audio);
            voiceClient.VoiceReceive += (ssrc, data) => stream.WriteAsync(data);
            await RespondAsync(InteractionCallback.ChannelMessageWithSource("Echo!"));
        }
        else
            throw new("You are not connected to any voice channel!");
    }
}
