using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a button component that links to an external URL within Discord.
/// </summary>
public class LinkButton(JsonButtonComponent jsonModel) : ICustomizableButton, IJsonModel<JsonButtonComponent>
{
    JsonButtonComponent IJsonModel<JsonButtonComponent>.JsonModel => jsonModel;

    /// <summary>
    /// The unique integer ID of the component.
    /// </summary>
    public int Id => jsonModel.Id;

    /// <summary>
    /// The target URL that the button opens when clicked.
    /// </summary>
    public string Url => jsonModel.Url!;

    /// <summary>
    /// The text label displayed on the link button.
    /// </summary>
    public string? Label => jsonModel.Label;

    /// <summary>
    /// The emoji displayed alongside or instead of the button text label.
    /// </summary>
    public EmojiReference? Emoji { get; } = jsonModel.Emoji is { } emoji ? new EmojiReference(emoji) : null;

    /// <summary>
    /// Whether the link button is disabled and cannot be clicked.
    /// </summary>
    public bool Disabled => jsonModel.Disabled.GetValueOrDefault();
}
