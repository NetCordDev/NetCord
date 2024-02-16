namespace NetCord;

public class GuildThreadMetadata(JsonModels.JsonGuildThreadMetadata jsonModel) : IJsonModel<JsonModels.JsonGuildThreadMetadata>
{
    JsonModels.JsonGuildThreadMetadata IJsonModel<JsonModels.JsonGuildThreadMetadata>.JsonModel => jsonModel;

    public bool Archived => jsonModel.Archived;
    public int AutoArchiveDuration => jsonModel.AutoArchiveDuration;
    public DateTimeOffset ArchiveTimestamp => jsonModel.ArchiveTimestamp;
    public bool Locked => jsonModel.Locked;
    public bool? Invitable => jsonModel.Invitable;
}
