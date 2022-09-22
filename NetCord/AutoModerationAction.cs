using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationAction : IJsonModel<JsonAutoModerationAction>
{
    JsonAutoModerationAction IJsonModel<JsonAutoModerationAction>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationAction _jsonModel;

    public AutoModerationAction(JsonAutoModerationAction jsonModel)
    {
        _jsonModel = jsonModel;
        if (_jsonModel.Metadata != null)
            Metadata = new(_jsonModel.Metadata);
    }

    public AutoModerationActionType Type => _jsonModel.Type;

    public AutoModerationActionMetadata? Metadata { get; }
}
