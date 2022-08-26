namespace NetCord.Gateway.Voice;

[Flags]
public enum SpeakingFlags
{
    Microphone = 1 << 0,
    Soundshare = 1 << 1,
    Priority = 1 << 2,
}
