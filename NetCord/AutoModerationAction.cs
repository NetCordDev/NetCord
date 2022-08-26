using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationAction : IJsonModel<JsonAutoModerationAction>
{
    JsonAutoModerationAction IJsonModel<JsonAutoModerationAction>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationAction _jsonModel;

    public AutoModerationAction(JsonAutoModerationAction jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public AutoModerationActionType Type => _jsonModel.Type;

    public JsonAutoModerationActionMetadata? Metadata => _jsonModel.Metadata;
}
