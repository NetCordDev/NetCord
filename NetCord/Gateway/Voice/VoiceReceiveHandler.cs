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
    public delegate void VoiceReceiveDelegate(VoiceReceiveData data);
    public event VoiceReceiveDelegate? VoiceReceive;
#endif

    public abstract bool RequiresExternalSocketAddress { get; }

    public abstract void HandlePacket(RtpPacket packet);

    protected void InvokeVoiceReceive(VoiceReceiveData data)
    {
#if !BUFFERED_HANDLER_TEST
        _client!.InvokeVoiceReceive(data);
#else
        VoiceReceive?.Invoke(data);
#endif
    }
}
