using NetCord.Gateway;
using NetCord.Gateway.Voice;

namespace MyBot;

internal static class Examples
{
    public static async Task ConnectWithLongerTimeoutAsync(GatewayClient client,
                                                           ulong guildId,
                                                           ulong channelId,
                                                           VoiceClientConfiguration? configuration)
    {
        var voiceClient = await client.JoinVoiceChannelAsync(guildId, channelId, configuration, TimeSpan.FromSeconds(10));
    }

    public static async Task KeepAliveAsync(VoiceClient voiceClient, CancellationToken cancellationToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync(cancellationToken))
            await voiceClient.SendDatagramAsync(ReadOnlyMemory<byte>.Empty, cancellationToken);
    }
}
