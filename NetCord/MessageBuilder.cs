using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public class MessageBuilder
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("tts")]
        public bool? Tts { get; set; }

        [JsonIgnore]
        public List<MessageFile>? Files { get; set; }

        [JsonPropertyName("embeds")]
        public List<Embed>? Embeds { get; set; }

        [JsonPropertyName("allowed_mentions")]
        public AllowedMentions? AllowedMentions { get; set; }

        [JsonPropertyName("message_reference")]
        public Reference? MessageReference { get; set; }

        [JsonPropertyName("components")]
        public List<Component>? Components { get; set; }

        [JsonPropertyName("sticker_ids")]
        public List<DiscordId>? StickerIds { get; set; }

        public BuiltMessage Build()
        {
            MemoryStream stream = new();
            JsonSerializer.Serialize(stream, this);
            stream.Position = 0;
            MultipartFormDataContent content = new();
            content.Add(new StreamContent(stream), "payload_json");
            for (var i = 0; i < Files?.Count; i++)
            {
                MessageFile file = Files[i];
                StreamContent c = new(file.Stream);
                content.Add(c, i.ToString(), file.FileName);
            }
            return new(content);
        }
    }
}
