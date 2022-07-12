namespace NetCord;

public class GuildThreadMetadata : IJsonModel<JsonModels.JsonGuildThreadMetadata>
{
    JsonModels.JsonGuildThreadMetadata IJsonModel<JsonModels.JsonGuildThreadMetadata>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildThreadMetadata _jsonModel;

    public bool Archived => _jsonModel.Archived;
    public int AutoArchiveDuration => _jsonModel.AutoArchiveDuration;
    public DateTimeOffset ArchiveTimestamp => _jsonModel.ArchiveTimestamp;
    public bool Locked => _jsonModel.Locked;
    public bool? Invitable => _jsonModel.Invitable;

    public GuildThreadMetadata(JsonModels.JsonGuildThreadMetadata jsonModel)
    {
        _jsonModel = jsonModel;
    }
}