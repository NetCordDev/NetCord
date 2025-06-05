namespace NetCord.Gateway.Voice;

public readonly struct VoicePacketHandlingResult(ushort framesMissed, bool handle)
{
    public ushort FramesMissed => framesMissed;

    public bool Handle => handle;
}
