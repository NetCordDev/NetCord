using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a reaction to a message.
/// </summary>
public class MessageReaction(JsonMessageReaction jsonModel)
{
    /// <summary>
    /// The total number of times this reaction has been applied, including super reactions.
    /// </summary>
    public int Count { get; } = jsonModel.Count;

    /// <summary>
    /// Contains additional information on the reaction count.
    /// </summary>
    public MessageReactionCountDetails CountDetails { get; } = new(jsonModel.CountDetails);

    /// <summary>
    /// Whether the current user applied this reaction.
    /// </summary>
    public bool Me { get; } = jsonModel.Me;

    /// <summary>
    /// Whether the current user applied this reaction as a super reaction.
    /// </summary>
    public bool MeBurst { get; } = jsonModel.MeBurst;

    /// <summary>
    /// A minimal emoji object, containing information about the reacted emoji.
    /// </summary>
    public MessageReactionEmoji Emoji { get; } = new(jsonModel.Emoji);

    /// <summary>
    /// An array of colors used for super reactions.
    /// </summary>
    public IReadOnlyList<Color> BurstColors { get; } = jsonModel.BurstColors;
}
