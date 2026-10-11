using NetCord.JsonModels;

namespace NetCord;

public class AutoModerationAction(JsonAutoModerationAction jsonModel)
{
    public AutoModerationActionType Type { get; } = jsonModel.Type;

    public AutoModerationActionMetadata? Metadata { get; } = jsonModel.Metadata is { } metadata ? new(metadata) : null;
}
