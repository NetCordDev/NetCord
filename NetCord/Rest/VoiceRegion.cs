namespace NetCord;

public class VoiceRegion
{
    private readonly JsonModels.JsonVoiceRegion _jsonEntity;

    public string Id => _jsonEntity.Id;

    public string Name => _jsonEntity.Name;

    public bool Optimal => _jsonEntity.Optimal;

    public bool Deprecated => _jsonEntity.Deprecated;

    public bool Custom => _jsonEntity.Custom;

    internal VoiceRegion(JsonModels.JsonVoiceRegion jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}