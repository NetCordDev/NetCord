namespace NetCord.Gateway.Voice;

public interface IVoiceReceiveHandler
{
    public bool RequiresExternalSocketAddress { get; }

    public VoicePacketHandlingResult HandlePacket(RtpPacket packet);
}
