namespace NetCord.Rest;

public class VoiceRegion : IJsonModel<JsonModels.JsonVoiceRegion>
{
    JsonModels.JsonVoiceRegion IJsonModel<JsonModels.JsonVoiceRegion>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonVoiceRegion _jsonModel;

    public string Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public bool Optimal => _jsonModel.Optimal;

    public bool Deprecated => _jsonModel.Deprecated;

    public bool Custom => _jsonModel.Custom;

    public VoiceRegion(JsonModels.JsonVoiceRegion jsonModel)
    {
        _jsonModel = jsonModel;
    }
}