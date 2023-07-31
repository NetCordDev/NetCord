using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(MessageButtonConverter))]
public abstract partial class ButtonProperties
{
    /// <summary>
    /// Style of the button.
    /// </summary>
    [JsonPropertyName("style")]
    public ButtonStyle Style { get; set; }

    /// <summary>
    /// Type of the component.
    /// </summary>
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Button;

    /// <summary>
    /// Text that appears on the button (max 80 characters).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    /// Emoji that appears on the button.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }

    /// <summary>
    /// Whether the button is disabled.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    /// <param name="style">Style of the button.</param>
    protected ButtonProperties(string label, ButtonStyle style)
    {
        Style = style;
        Label = label;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="emoji">Emoji that appears on the button.</param>
    /// <param name="style">Style of the button.</param>
    protected ButtonProperties(EmojiProperties emoji, ButtonStyle style)
    {
        Style = style;
        Emoji = emoji;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="label">Text that appears on the button (max 80 characters).</param>
    /// <param name="emoji">Emoji that appears on the button.</param>
    /// <param name="style">Style of the button.</param>
    protected ButtonProperties(string label, EmojiProperties emoji, ButtonStyle style)
    {
        Style = style;
        Label = label;
        Emoji = emoji;
    }

    public class MessageButtonConverter : JsonConverter<ButtonProperties>
    {
        public override ButtonProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, ButtonProperties button, JsonSerializerOptions options)
        {
            switch (button)
            {
                case ActionButtonProperties actionButton:
                    JsonSerializer.Serialize(writer, actionButton, ActionButtonProperties.ActionButtonPropertiesSerializerContext.WithOptions.ActionButtonProperties);
                    break;
                case LinkButtonProperties linkButton:
                    JsonSerializer.Serialize(writer, linkButton, LinkButtonProperties.LinkButtonPropertiesSerializerContext.WithOptions.LinkButtonProperties);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid {nameof(ButtonProperties)} value.");
            }
        }
    }
}
