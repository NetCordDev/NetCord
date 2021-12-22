using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord
{
    public class InteractionMessage
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("tts")]
        public bool Tts { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("embeds")]
        public List<MessageEmbed>? Embeds { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("allowed_mentions")]
        public AllowedMentions? AllowedMentions { get; set; }

        [JsonConverter(typeof(EphemeralConverter))]
        [JsonPropertyName("flags")]
        public bool Ephemeral { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("components")]
        public List<Component>? Components { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(JsonConverters.MessageAttachmentListConverter))]
        [JsonPropertyName("attachments")]
        public List<MessageAttachment>? Attachments { get; set; }

        private class EphemeralConverter : JsonConverter<bool>
        {
            public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
            public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            {
                if (value)
                    writer.WriteNumberValue((uint)MessageFlags.Ephemeral);
                else
                    writer.WriteNumberValue(0);
            }
        }
    }
}
