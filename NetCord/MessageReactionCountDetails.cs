using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents count information for a <see cref="MessageReaction"/> object.
/// </summary>
public class MessageReactionCountDetails(JsonMessageReactionCountDetails jsonModel)
{
    /// <summary>
    /// The number of applied super reactions.
    /// </summary>
    public int Burst { get; } = jsonModel.Burst;

    /// <summary>
    /// The number of applied normal reactions.
    /// </summary>
    public int Normal { get; } = jsonModel.Normal;
}
