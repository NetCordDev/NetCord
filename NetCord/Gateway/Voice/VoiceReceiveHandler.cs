namespace NetCord.Gateway.Voice;

public abstract class VoiceReceiveHandler
{
#if !BUFFERED_HANDLER_TEST
    private VoiceClient? _client;

    internal void Start(VoiceClient client)
    {
        if (Interlocked.CompareExchange(ref _client, client, null) is not null)
            throw new InvalidOperationException("This handler is already registered to a voice client.");
    }
#else
    public event Action<VoiceReceiveEventArgs>? VoiceReceive;
#endif

    public abstract bool RequiresExternalSocketAddress { get; }

    public abstract void Handle(VoiceReceiveContext context);

    protected void InvokeVoiceReceive(VoiceReceiveEventArgs eventArgs)
    {
#if !BUFFERED_HANDLER_TEST
        _client!.InvokeVoiceReceive(eventArgs);
#else
        VoiceReceive?.Invoke(eventArgs);
#endif
    }
}

public readonly ref struct VoiceReceiveContext(RtpPacket packet, ReadOnlySpan<byte> frame)
{
    public readonly RtpPacket Packet { get; } = packet;

    public readonly ReadOnlySpan<byte> Frame { get; } = frame;
}
