namespace NetCord.Gateway.Voice;

internal class VoiceInStream(VoiceClient client, uint ssrc, ulong userId) : VoiceStream
{
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => client.InvokeVoiceReceiveAsync(new(ssrc, userId, buffer));

    public override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException();
}
