using System.Text.Json.Serialization;

namespace NetCord;

public class ActionRowProperties : ComponentProperties
{
    [JsonPropertyName("components")]
    public IEnumerable<ButtonProperties> Buttons { get; }

    public ActionRowProperties(IEnumerable<ButtonProperties> buttons) : base(ComponentType.ActionRow)
    {
        Buttons = buttons;
    }
}