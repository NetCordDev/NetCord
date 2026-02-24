namespace NetCord.Gateway.Voice;

public class NullVoiceReceiveHandler : VoiceReceiveHandler
{
    public override bool RequiresExternalSocketAddress => false;

    public override void HandlePacket(RtpPacket packet)
    {
    }
}
