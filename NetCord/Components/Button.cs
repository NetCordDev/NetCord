using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(JsonConverters.ButtonConverter))]
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
    }
}
