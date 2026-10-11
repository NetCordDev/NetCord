namespace NetCord.Rest;

public class VoiceRegion(JsonModels.JsonVoiceRegion jsonModel)
{
    public string Id { get; } = jsonModel.Id;

    public string Name { get; } = jsonModel.Name;

    public bool Optimal { get; } = jsonModel.Optimal;

    public bool Deprecated { get; } = jsonModel.Deprecated;

    public bool Custom { get; } = jsonModel.Custom;
}
