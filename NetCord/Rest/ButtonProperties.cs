using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(MessageButtonConverter))]
public abstract class ButtonProperties
{
    [JsonPropertyName("style")]
    public ButtonStyle Style { get; }

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Button;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("label")]
    public string? Label { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    protected ButtonProperties(ButtonStyle style, string label)
    {
        Style = style;
        Label = label;
    }

    protected ButtonProperties(ButtonStyle style, EmojiProperties emoji)
    {
        Style = style;
        Emoji = emoji;
    }

    protected ButtonProperties(ButtonStyle style, string label, EmojiProperties emoji)
    {
        Style = style;
        Label = label;
        Emoji = emoji;
    }

    private class MessageButtonConverter : JsonConverter<ButtonProperties>
    {
        public override ButtonProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, ButtonProperties button, JsonSerializerOptions options)
        {
            if (button is ActionButtonProperties actionButton)
                JsonSerializer.Serialize(writer, actionButton, options);
            else if (button is LinkButtonProperties linkButton)
                JsonSerializer.Serialize(writer, linkButton, options);
        }
    }
}