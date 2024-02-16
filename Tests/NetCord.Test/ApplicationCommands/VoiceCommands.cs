using System.Diagnostics;

using NetCord.Gateway.Voice;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

public class VoiceCommands(Dictionary<ulong, SemaphoreSlim> joinSemaphores) : ApplicationCommandModule<SlashCommandContext>
{
    private async Task<VoiceClient> JoinAsync()
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
            voiceClient = await client.JoinVoiceChannelAsync(guild.Id, state.ChannelId.GetValueOrDefault(), new()
            {
                RedirectInputStreams = true,
            });
        }
        finally
        {
            semaphore.Release();
        }
        voiceClient.Log += m =>
        {
            Console.WriteLine(m);
            return default;
        };
        await voiceClient.StartAsync();
        await voiceClient.ReadyAsync;

        await voiceClient.EnterSpeakingStateAsync(SpeakingFlags.Microphone);

        return voiceClient;
    }

    [SlashCommand("play", "Plays music")]
    public async Task PlayAsync()
    {
        using var voiceClient = await JoinAsync();

        var outputStream = voiceClient.CreateOutputStream();
        using OpusEncodeStream opusEncodeStream = new(outputStream, PcmFormat.Float, VoiceChannels.Stereo, OpusApplication.Audio);

        var url = "https://www.mfiles.co.uk/mp3-downloads/beethoven-symphony6-1.mp3"; // 00:12:08
        //var url = "https://file-examples.com/storage/feee5c69f0643c59da6bf13/2017/11/file_example_MP3_700KB.mp3"; // 00:00:27
        await RespondAsync(InteractionCallback.Message($"Playing: {Path.GetFileNameWithoutExtension(url)}"));
        using var ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{url}\" -ac 2 -f f32le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
        })!;

        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(opusEncodeStream);
        await opusEncodeStream.FlushAsync();
    }

    [SlashCommand("echo", "Echo!")]
    public async Task EchoAsync()
    {
        using var voiceClient = await JoinAsync();

        using var outputStream = voiceClient.CreateOutputStream(false);
        await RespondAsync(InteractionCallback.Message("Echo!"));

        voiceClient.VoiceReceive += args => outputStream.WriteAsync(args.Frame);

        await Task.Delay(TimeSpan.FromMinutes(10));
    }
}
