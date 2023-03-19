namespace NetCord.Gateway.Voice;

internal class VoiceInStream : VoiceStream
{
    private readonly VoiceClient _client;
    private readonly uint _ssrc;
    private readonly ulong _userId;

    public VoiceInStream(VoiceClient client, uint ssrc, ulong userId)
    {
        _client = client;
        _ssrc = ssrc;
        _userId = userId;
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => _client.InvokeVoiceReceiveAsync(new(_ssrc, _userId, buffer));

    public override void Write(ReadOnlySpan<byte> buffer) => throw new NotSupportedException();
}
