using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationActionMetadata(JsonAutoModerationActionMetadata jsonModel) : IJsonModel<JsonAutoModerationActionMetadata>
{
    JsonAutoModerationActionMetadata IJsonModel<JsonAutoModerationActionMetadata>.JsonModel => throw new NotImplementedException();

    public ulong? ChannelId => jsonModel.ChannelId;
    public int? DurationSeconds => jsonModel.DurationSeconds;
    public string? CustomMessage => jsonModel.CustomMessage;
}
