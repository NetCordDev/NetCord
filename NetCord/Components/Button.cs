using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(ButtonConverter))]
    public abstract class Button
    {
        [JsonPropertyName("style")]
        public MessageButtonStyle Style { get; }

        [Browsable(false)]
        [JsonPropertyName("type")]
        public MessageComponentType ComponentType => MessageComponentType.Button;

        [JsonPropertyName("label")]
        public string Label { get; }

        [JsonPropertyName("emoji")]
        [JsonConverter(typeof(JsonConverters.ComponentEmojiConverter))]
        public DiscordId EmojiId { get; init; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; init; }

        protected Button(string label, MessageButtonStyle style)
        {
            Label = label;
            Style = style;
        }

        private class ButtonConverter : JsonConverter<Button>
        {
            public override Button Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
            public override void Write(Utf8JsonWriter writer, Button button, JsonSerializerOptions options)
            {
                if (button is ActionButton actionButton)
                    JsonSerializer.Serialize(writer, actionButton);
                else if (button is LinkButton linkButton)
                    JsonSerializer.Serialize(writer, linkButton);
            }
        }
    }
}
