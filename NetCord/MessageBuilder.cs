using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord
{
    public class MessageBuilder
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("tts")]
        public bool Tts { get; set; }

        [JsonConverter(typeof(MessageFileListConverter))]
        [JsonPropertyName("attachments")]
        public List<MessageFile>? Files { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("embeds")]
        public List<Embed>? Embeds { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("allowed_mentions")]
        public AllowedMentions? AllowedMentions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("message_reference")]
        public Reference? MessageReference { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("components")]
        public List<Component>? Components { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("sticker_ids")]
        public List<DiscordId>? StickerIds { get; set; }

        public BuiltMessage Build()
        {
            MultipartFormDataContent content = new();
            content.Add(new JsonContent(this), "payload_json");
            if (Files != null)
            {
                var count = Files.Count;
                for (var i = 0; i < count; i++)
                {
                    MessageFile file = Files[i];
                    content.Add(new SeekableStreamContent(file._stream), $"files[{i}]", file.FileName);
                }
            }
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(content.ReadAsStringAsync().Result);
            Console.ResetColor();
            return new(content);
        }

        private class MessageFileListConverter : JsonConverter<List<MessageFile>>
        {
            public override List<MessageFile>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
            public override void Write(Utf8JsonWriter writer, List<MessageFile> value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                int count = value.Count;
                for (int i = 0; i < count; i++)
                {
                    var file = value[i];
                    if (file.Description != null)
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("id", i);
                        writer.WriteString("description", file.Description);
                        writer.WriteString("filename", file.FileName);
                        writer.WriteEndObject();
                    }
                }
                writer.WriteEndArray();
            }
        }
    }
}
