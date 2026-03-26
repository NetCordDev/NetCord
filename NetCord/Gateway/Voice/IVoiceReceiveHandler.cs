namespace NetCord.Gateway.Voice;

public interface IVoiceReceiveHandler
{
    public bool RequiresExternalSocketAddress { get; }

    public VoicePacketHandlingResult HandlePacket(VoiceClient client, RtpPacket packet);
}
