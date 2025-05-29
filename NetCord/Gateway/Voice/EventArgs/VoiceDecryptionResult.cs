namespace NetCord.Gateway.Voice;

public readonly struct VoiceDecryptionResult(int frameIndex)
{
    public int FrameIndex => frameIndex;
}

public readonly struct ContinuousVoiceDecryptionResult(int frameIndex, ushort framesMissed, ContinuousVoiceDecryptionStatus status)
{
    public int FrameIndex => frameIndex;
    public ushort FramesMissed => framesMissed;
    public ContinuousVoiceDecryptionStatus Status => status;
}
