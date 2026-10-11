using NetCord.Gateway.Voice.JsonModels;

namespace NetCord.Gateway.Voice;

public class SpeakingEventArgs(JsonSpeaking jsonModel)
{
    public ulong UserId { get; } = jsonModel.UserId;

    public uint Ssrc { get; } = jsonModel.Ssrc;

    public SpeakingFlags Speaking { get; } = jsonModel.Speaking;
}
