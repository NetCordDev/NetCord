namespace NetCord.Rest;

public class VoiceRegion(JsonModels.JsonVoiceRegion jsonModel) : IJsonModel<JsonModels.JsonVoiceRegion>
{
    JsonModels.JsonVoiceRegion IJsonModel<JsonModels.JsonVoiceRegion>.JsonModel => jsonModel;

    public string Id => jsonModel.Id;

    public string Name => jsonModel.Name;

    public bool Optimal => jsonModel.Optimal;

    public bool Deprecated => jsonModel.Deprecated;

    public bool Custom => jsonModel.Custom;
}
