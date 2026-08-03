namespace NetCord;

/// <summary>
/// Contains information about a <see cref="MessageReaction"/>'s emoji.
/// </summary>
public class MessageReactionEmoji(JsonModels.JsonEmoji jsonModel) : IJsonModel<JsonModels.JsonEmoji>
{
    JsonModels.JsonEmoji IJsonModel<JsonModels.JsonEmoji>.JsonModel => jsonModel;

    /// <summary>
    /// <inheritdoc cref="CustomEmoji.Id"/> Can be <see langword="null"/> for standard emoji.
    /// </summary>
    public ulong? Id => jsonModel.Id;

    /// <summary>
    /// The emoji's name.
    /// </summary>
    public string? Name => jsonModel.Name;

    /// <summary>
    /// Whether the emoji is animated.
    /// </summary>
    public bool Animated => jsonModel.Animated;
}
