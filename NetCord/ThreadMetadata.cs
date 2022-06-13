namespace NetCord;

public class ThreadMetadata : IJsonModel<JsonModels.JsonThreadMetadata>
{
    JsonModels.JsonThreadMetadata IJsonModel<JsonModels.JsonThreadMetadata>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonThreadMetadata _jsonModel;

    public bool Archived => _jsonModel.Archived;
    public int AutoArchiveDuration => _jsonModel.AutoArchiveDuration;
    public DateTimeOffset ArchiveTimestamp => _jsonModel.ArchiveTimestamp;
    public bool Locked => _jsonModel.Locked;
    public bool? Invitable => _jsonModel.Invitable;

    public ThreadMetadata(JsonModels.JsonThreadMetadata jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
