namespace NetCord;

/// <summary>
/// Represents a lightweight reference to an emoji.
/// </summary>
public class EmojiReference(JsonModels.JsonEmoji jsonModel)
{
    /// <inheritdoc cref="CustomEmoji.Id"/>
    public ulong? Id { get; } = jsonModel.Id;

    /// <inheritdoc cref="Emoji.Name"/>
    public string Name { get; } = jsonModel.Name!;

    /// <inheritdoc cref="Emoji.Animated"/>
    public bool? Animated { get; } = jsonModel.Animated;
}
