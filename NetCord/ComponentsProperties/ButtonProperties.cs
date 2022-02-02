using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(MessageButtonConverter))]
public abstract class ButtonProperties
{
    [JsonPropertyName("style")]
    public ButtonStyle Style { get; }

    [Browsable(false)]
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Button;

    [JsonPropertyName("label")]
    public string Label { get; }

    [JsonPropertyName("emoji")]
    [JsonConverter(typeof(JsonConverters.ComponentEmojiConverter))]
    public DiscordId? EmojiId { get; init; }

    [JsonPropertyName("disabled")]
    public bool Disabled { get; init; }

    protected ButtonProperties(string label, ButtonStyle style)
    {
        Label = label;
        Style = style;
    }

    private class MessageButtonConverter : JsonConverter<ButtonProperties>
    {
        public override ButtonProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, ButtonProperties button, JsonSerializerOptions options)
        {
            if (button is ActionButtonProperties actionButton)
                JsonSerializer.Serialize(writer, actionButton);
            else if (button is LinkButtonProperties linkButton)
                JsonSerializer.Serialize(writer, linkButton);
        }
    }
}