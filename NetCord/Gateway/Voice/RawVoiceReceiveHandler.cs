namespace NetCord.Gateway.Voice;

public sealed class RawVoiceReceiveHandler : VoiceReceiveHandler
{
    public override bool RequiresExternalSocketAddress => true;

    public override void HandlePacket(RtpPacket packet)
    {
        if (packet.PayloadType is not 0x78)
            return;

        InvokeVoiceReceive(VoiceReceiveData.FromPacket(packet));
    }
}
