namespace NetCord.Gateway.Voice;

public static class GatewayClientExtensions
{
    /// <summary>
    /// Joins a voice channel.
    /// </summary>
    /// <param name="client">The <see cref="GatewayClient"/> instance.</param>
    /// <param name="guildId">The ID of the guild containing the channel.</param>
    /// <param name="channelId">The ID of the voice channel to join.</param>
    /// <param name="configuration">Configuration settings for the <see cref="VoiceClient"/>.</param>
    /// <param name="timeout">The maximum amount of time to wait for the voice state and server update events. If not specified, a default timeout of 5 seconds is used.</param>
    /// <param name="timeProvider">The <see cref="TimeProvider"/> to use for measuring the timeout. If not specified, <see cref="TimeProvider.System"/> is used.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <remarks>This method is not thread safe and should not be used concurrently for the same <paramref name="guildId"/>.</remarks>
    /// <returns>A task, the result of which is an unconnected <see cref="VoiceClient"/> instance.</returns>
    public static async Task<VoiceClient> JoinVoiceChannelAsync(this GatewayClient client,
                                                                ulong guildId,
                                                                ulong channelId,
                                                                VoiceClientConfiguration? configuration = null,
                                                                TimeSpan? timeout = null,
                                                                TimeProvider? timeProvider = null,
                                                                CancellationToken cancellationToken = default)
    {
        var userId = client.Id;

        VoiceState? state = null;
        VoiceServerUpdateEventArgs? server = null;

        TaskCompletionSource eventsTaskCompletionSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

        var voiceStateUpdateHandler = HandleVoiceStateUpdateAsync;
        var voiceServerUpdateHandler = HandleVoiceServerUpdateAsync;

        client.VoiceStateUpdate += voiceStateUpdateHandler;
        client.VoiceServerUpdate += voiceServerUpdateHandler;

        try
        {
            await client.UpdateVoiceStateAsync(new(guildId, channelId), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            client.VoiceStateUpdate -= voiceStateUpdateHandler;
            client.VoiceServerUpdate -= voiceServerUpdateHandler;
            throw;
        }

        try
        {
            await eventsTaskCompletionSource.Task.WaitAsync(timeout ?? new(5 * TimeSpan.TicksPerSecond),
                                                            timeProvider ?? TimeProvider.System,
                                                            cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            client.VoiceStateUpdate -= voiceStateUpdateHandler;
            client.VoiceServerUpdate -= voiceServerUpdateHandler;
        }

        if (server!.Endpoint is not { } endpoint)
            throw new InvalidOperationException("The voice server is unavailable.");

        return new(userId, state!.SessionId, endpoint, guildId, channelId, server.Token, configuration);

        ValueTask HandleVoiceStateUpdateAsync(VoiceState arg)
        {
            if (arg.UserId == userId && arg.ChannelId == channelId && state is null)
            {
                state = arg;

                if (server is not null)
                    eventsTaskCompletionSource.TrySetResult();
            }

            return default;
        }

        ValueTask HandleVoiceServerUpdateAsync(VoiceServerUpdateEventArgs arg)
        {
            if (arg.GuildId == guildId && server is null)
            {
                server = arg;

                if (state is not null)
                    eventsTaskCompletionSource.TrySetResult();
            }

            return default;
        }
    }
}
