using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;

namespace MyBot;

internal static class Examples
{
    public static async Task ConnectToVoiceAsync(GatewayClient client, ulong guildId, ulong channelId)
    {
        var voiceClient = await client.JoinVoiceChannelAsync(guildId, channelId, new VoiceClientConfiguration
        {
            Logger = new ConsoleLogger(),
        });

        await voiceClient.StartAsync();
    }

    public static async Task LeaveVoiceAsync(GatewayClient client, VoiceClient voiceClient, ulong guildId)
    {
        await voiceClient.CloseAsync();
        voiceClient.Dispose();

        await client.UpdateVoiceStateAsync(new(guildId, channelId: null));
    }
}
