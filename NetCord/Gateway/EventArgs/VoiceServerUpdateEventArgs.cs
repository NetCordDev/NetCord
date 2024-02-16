namespace NetCord.Gateway;

public class VoiceServerUpdateEventArgs(JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs>
{
    JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs>.JsonModel => jsonModel;

    public string Token => jsonModel.Token;

    public ulong GuildId => jsonModel.GuildId;

    public string? Endpoint => jsonModel.Endpoint;
}
