using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents an option in a string select menu component.
/// </summary>
public class StringMenuSelectOption(JsonStringMenuSelectOption jsonModel) : IJsonModel<JsonStringMenuSelectOption>
{
    JsonStringMenuSelectOption IJsonModel<JsonStringMenuSelectOption>.JsonModel => jsonModel;

    /// <summary>
    /// The user-facing name of the option.
    /// </summary>
    public string Label => jsonModel.Label;

    /// <summary>
    /// The dev-defined value of the option.
    /// </summary>
    public string Value => jsonModel.Value;

    /// <summary>
    /// An additional description of the option.
    /// </summary>
    public string? Description => jsonModel.Description;

    /// <summary>
    /// The emoji displayed on the option.
    /// </summary>
    public EmojiReference? Emoji { get; } = jsonModel.Emoji is { } emoji ? new(emoji) : null;

    /// <summary>
    /// Whether this option is selected by default.
    /// </summary>
    public bool Default => jsonModel.Default;
}
