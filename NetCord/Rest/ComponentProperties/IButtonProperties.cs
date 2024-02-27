using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(MessageButtonConverter))]
public partial interface IButtonProperties
{
    /// <summary>
    /// Style of the button.
    /// </summary>
    public ButtonStyle Style { get; set; }

    /// <summary>
    /// Type of the component.
    /// </summary>
    public ComponentType ComponentType { get; }

    /// <summary>
    /// Text that appears on the button (max 80 characters).
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Emoji that appears on the button.
    /// </summary>
    public EmojiProperties? Emoji { get; set; }

    /// <summary>
    /// Whether the button is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    public class MessageButtonConverter : JsonConverter<IButtonProperties>
    {
        public override IButtonProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, IButtonProperties button, JsonSerializerOptions options)
        {
            switch (button)
            {
                case ButtonProperties actionButton:
                    JsonSerializer.Serialize(writer, actionButton, Serialization.Default.ButtonProperties);
                    break;
                case LinkButtonProperties linkButton:
                    JsonSerializer.Serialize(writer, linkButton, Serialization.Default.LinkButtonProperties);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid {nameof(IButtonProperties)} value.");
            }
        }
    }
}
