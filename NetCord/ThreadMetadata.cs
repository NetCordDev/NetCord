namespace NetCord;

public class ThreadMetadata
{
    private readonly JsonModels.JsonThreadMetadata _jsonEntity;

    public bool Archived => _jsonEntity.Archived;
    public int AutoArchiveDuration => _jsonEntity.AutoArchiveDuration;
    public DateTimeOffset ArchiveTimestamp => _jsonEntity.ArchiveTimestamp;
    public bool Locked => _jsonEntity.Locked;
    public bool? Invitable => _jsonEntity.Invitable;

    internal ThreadMetadata(JsonModels.JsonThreadMetadata jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }
}
