namespace NetCord.Gateway.Voice;

public class NullVoiceReceiveHandler : IVoiceReceiveHandler
{
    public static NullVoiceReceiveHandler Instance { get; } = new();

    private NullVoiceReceiveHandler()
    {
    }

    public bool RequiresExternalSocketAddress => false;

    public VoicePacketHandlingResult HandlePacket(VoiceClient client, RtpPacket packet)
    {
        return default;
    }
}
