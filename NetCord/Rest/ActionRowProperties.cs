using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="buttons">Buttons of the action row (max 5).</param>
public partial class ActionRowProperties(IEnumerable<ButtonProperties> buttons) : ComponentProperties(ComponentType.ActionRow)
{
    /// <summary>
    /// Buttons of the action row (max 5).
    /// </summary>
    [JsonPropertyName("components")]
    public IEnumerable<ButtonProperties> Buttons { get; set; } = buttons;
}
