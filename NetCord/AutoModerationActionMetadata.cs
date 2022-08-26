using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationActionMetadata : IJsonModel<JsonAutoModerationActionMetadata>
{
    JsonAutoModerationActionMetadata IJsonModel<JsonAutoModerationActionMetadata>.JsonModel => throw new NotImplementedException();
    private readonly JsonAutoModerationActionMetadata _jsonModel;

    public AutoModerationActionMetadata(JsonAutoModerationActionMetadata jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake? ChannelId => _jsonModel.ChannelId;

    public int? DurationSeconds => _jsonModel.DurationSeconds;
}
