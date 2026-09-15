using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents an action that is executed when an auto moderation rule is triggered.
/// </summary>
public class AutoModerationAction(JsonAutoModerationAction jsonModel) : IJsonModel<JsonAutoModerationAction>
{
    JsonAutoModerationAction IJsonModel<JsonAutoModerationAction>.JsonModel => jsonModel;

    /// <summary>
    /// The type of action to be executed.
    /// </summary>
    public AutoModerationActionType Type => jsonModel.Type;

    /// <summary>
    /// Additional metadata needed for specific action types, or <see langword="null"/> if none is required.
    /// </summary>
    public AutoModerationActionMetadata? Metadata { get; } = jsonModel.Metadata is { } metadata ? new(metadata) : null;
}
