using NetCord.Gateway.Voice.JsonModels;

namespace NetCord.Gateway.Voice;

public class SpeakingEventArgs(JsonSpeaking jsonModel) : IJsonModel<JsonSpeaking>
{
    JsonSpeaking IJsonModel<JsonSpeaking>.JsonModel => jsonModel;

    public ulong UserId => jsonModel.UserId;

    public uint Ssrc => jsonModel.Ssrc;

    public SpeakingFlags Speaking => jsonModel.Speaking;
}
