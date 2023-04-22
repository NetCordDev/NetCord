namespace NetCord.Gateway.Voice;

public static class GatewayClientExtensions
{
    /// <summary>
    /// Joins a voice channel.
    /// </summary>
    /// <param name="client">The <see cref="GatewayClient"/> instance.</param>
    /// <param name="userId">The id of the user represented by <see cref="GatewayClient"/>.</param>
    /// <param name="guildId">The id of the guild containing the channel.</param>
    /// <param name="channelId">The id of the voice channel to join.</param>
    /// <param name="configuration">Configuration settings for the <see cref="VoiceClient"/>.</param>
    /// <param name="cancellationToken">Cancellation token for the operation. If not cancellable, the default timeout of 2 seconds is used.</param>
    /// <remarks>This method is not thread safe and should not be used concurrently for the same <paramref name="guildId"/>.</remarks>
    /// <returns>A task, the result of which is an unconnected <see cref="VoiceClient"/> instance.</returns>
    public static async Task<VoiceClient> JoinVoiceChannelAsync(this GatewayClient client, ulong userId, ulong guildId, ulong channelId, VoiceClientConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
        TaskCompletionSource<VoiceState> stateTaskCompletionSource = new();
        TaskCompletionSource<VoiceServerUpdateEventArgs> serverTaskCompletionSource = new();

        var voiceStateUpdateHandler = HandleVoiceStateUpdateAsync;
        var voiceServerUpdateHandler = HandleVoiceServerUpdateAsync;

        client.VoiceStateUpdate += voiceStateUpdateHandler;
        client.VoiceServerUpdate += voiceServerUpdateHandler;

        try
        {
            await client.UpdateVoiceStateAsync(new(guildId, channelId)).ConfigureAwait(false);
        }
        catch
        {
            client.VoiceStateUpdate -= voiceStateUpdateHandler;
            client.VoiceServerUpdate -= voiceServerUpdateHandler;
            throw;
        }

        VoiceState state;
        VoiceServerUpdateEventArgs server;
        if (cancellationToken.CanBeCanceled)
            (state, server) = await WaitForEventsAsync(cancellationToken).ConfigureAwait(false);
        else
        {
            using CancellationTokenSource tokenSource = new(2000);
            (state, server) = await WaitForEventsAsync(tokenSource.Token).ConfigureAwait(false);
        }

        return new(userId, state.SessionId, server.Endpoint!, guildId, server.Token, configuration);

        ValueTask HandleVoiceStateUpdateAsync(VoiceState arg)
        {
            if (arg.UserId == userId && arg.ChannelId == channelId)
                stateTaskCompletionSource.TrySetResult(arg);
            return default;
        }

        ValueTask HandleVoiceServerUpdateAsync(VoiceServerUpdateEventArgs arg)
        {
            if (arg.GuildId == guildId)
                serverTaskCompletionSource.TrySetResult(arg);
            return default;
        }

        async Task<(VoiceState, VoiceServerUpdateEventArgs)> WaitForEventsAsync(CancellationToken cancellationToken)
        {
            VoiceState state;
            try
            {
                state = await stateTaskCompletionSource.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                client.VoiceServerUpdate -= voiceServerUpdateHandler;
                throw;
            }
            finally
            {
                client.VoiceStateUpdate -= voiceStateUpdateHandler;
            }

            VoiceServerUpdateEventArgs server;
            try
            {
                server = await serverTaskCompletionSource.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                client.VoiceServerUpdate -= voiceServerUpdateHandler;
            }
            return (state, server);
        }
    }
}
