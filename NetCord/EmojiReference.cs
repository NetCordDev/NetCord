namespace NetCord;

/// <summary>
/// Represents a lightweight reference to an emoji.
/// </summary>
public class EmojiReference(JsonModels.JsonEmoji jsonModel) : IJsonModel<JsonModels.JsonEmoji>
{
    JsonModels.JsonEmoji IJsonModel<JsonModels.JsonEmoji>.JsonModel => jsonModel;

    /// <inheritdoc cref="CustomEmoji.Id"/>
    public ulong? Id => jsonModel.Id;

    /// <inheritdoc cref="Emoji.Name"/>
    public string Name => jsonModel.Name!;

    /// <inheritdoc cref="Emoji.Animated"/>
    public bool Animated => jsonModel.Animated;
}
