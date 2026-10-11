using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Contains information about a <see cref="MessageReaction"/>'s emoji.
/// </summary>
public class MessageReactionEmoji(JsonEmoji jsonModel)
{
    /// <summary>
    /// <inheritdoc cref="CustomEmoji.Id"/> Always <see langword="null"/> for standard emoji.
    /// </summary>
    public ulong? Id { get; } = jsonModel.Id;

    /// <summary>
    /// The emoji's name.
    /// </summary>
    public string? Name { get; } = jsonModel.Name;

    /// <summary>
    /// Whether the emoji is animated.
    /// </summary>
    public bool? Animated { get; } = jsonModel.Animated;
}
