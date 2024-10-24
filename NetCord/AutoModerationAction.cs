using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationAction : IJsonModel<JsonAutoModerationAction>
{
    public AutoModerationAction(JsonAutoModerationAction jsonModel)
    {
        _jsonModel = jsonModel;

        var metadata = jsonModel.Metadata;
        if (metadata is not null)
            Metadata = new(metadata);
    }

    JsonAutoModerationAction IJsonModel<JsonAutoModerationAction>.JsonModel => _jsonModel;
    private readonly JsonAutoModerationAction _jsonModel;

    public AutoModerationActionType Type => _jsonModel.Type;

    public AutoModerationActionMetadata? Metadata { get; }
}
