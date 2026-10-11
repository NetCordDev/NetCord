using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class VoiceServerUpdateEventArgs(JsonVoiceServerUpdateEventArgs jsonModel)
{
    public string Token { get; } = jsonModel.Token;

    public ulong GuildId { get; } = jsonModel.GuildId;

    public string? Endpoint { get; } = jsonModel.Endpoint;
}
