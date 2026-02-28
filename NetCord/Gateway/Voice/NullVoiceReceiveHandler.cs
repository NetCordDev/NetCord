namespace NetCord.Gateway.Voice;

public class NullVoiceReceiveHandler : VoiceReceiveHandler
{
    public override bool RequiresExternalSocketAddress => false;

    public override void Handle(VoiceReceiveContext context)
    {
    }
}
