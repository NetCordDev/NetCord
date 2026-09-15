using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a clickable button component within Discord.
/// </summary>
public class Button(JsonButtonComponent jsonModel) : IInteractiveComponent, ICustomizableButton, IJsonModel<JsonButtonComponent>
{
    JsonButtonComponent IJsonModel<JsonButtonComponent>.JsonModel => jsonModel;

    /// <summary>
    /// The unique integer ID of the component.
    /// </summary>
    public int Id => jsonModel.Id;

    /// <summary>
    /// The developer-defined identifier for the button.
    /// </summary>
    public string CustomId => jsonModel.CustomId!;

    /// <summary>
    /// The style and color appearance of the button.
    /// </summary>
    public ButtonStyle Style => jsonModel.Style;

    /// <summary>
    /// The text text label displayed on the button.
    /// </summary>
    public string? Label => jsonModel.Label;

    /// <summary>
    /// The emoji displayed alongside or instead of the button text label.
    /// </summary>
    public EmojiReference? Emoji { get; } = jsonModel.Emoji is { } emoji ? new EmojiReference(emoji) : null;

    /// <summary>
    /// Whether the button is disabled and cannot be clicked.
    /// </summary>
    public bool Disabled => jsonModel.Disabled.GetValueOrDefault();
}
