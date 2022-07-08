namespace NetCord.Gateway;

public class VoiceServerUpdateEventArgs : IJsonModel<JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs>
{
    JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs _jsonModel;

    public VoiceServerUpdateEventArgs(JsonModels.EventArgs.JsonVoiceServerUpdateEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string Token => _jsonModel.Token;

    public Snowflake GuildId => _jsonModel.GuildId;

    public string? Endpoint => _jsonModel.Endpoint;
}