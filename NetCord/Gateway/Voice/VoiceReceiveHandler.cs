namespace NetCord.Gateway.Voice;

public abstract class VoiceReceiveHandler
{
    private VoiceClient? _client;

    internal void Start(VoiceClient client)
    {
        if (Interlocked.CompareExchange(ref _client, client, null) is not null)
            throw new InvalidOperationException("This handler is already registered to a voice client.");
    }

    public abstract bool RequiresExternalSocketAddress { get; }

    public abstract void Handle(VoiceReceiveContext context);

    protected void InvokeVoiceReceive(VoiceReceiveEventArgs eventArgs)
    {
        _client!.InvokeVoiceReceive(eventArgs);
    }
}

public readonly ref struct VoiceReceiveContext(RtpPacket packet, ReadOnlySpan<byte> frame)
{
    public readonly RtpPacket Packet { get; } = packet;

    public readonly ReadOnlySpan<byte> Frame { get; } = frame;
}
