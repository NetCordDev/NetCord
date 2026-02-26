namespace NetCord.Gateway.Voice;

public sealed class RawVoiceReceiveHandler : VoiceReceiveHandler
{
    public override bool RequiresExternalSocketAddress => true;

    public override void Handle(VoiceReceiveContext context)
    {
        var packet = context.Packet;
        InvokeVoiceReceive(VoiceReceiveEventArgs.Delivered(context.Frame, packet.Ssrc, packet.Timestamp, packet.SequenceNumber));
    }
}
