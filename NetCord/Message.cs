using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord
{
    public class Message
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("tts")]
        public bool Tts { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(JsonConverters.MessageAttachmentListConverter))]
        [JsonPropertyName("attachments")]
        public List<MessageAttachment>? Attachments { get; set; }

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

        internal MultipartFormDataContent Build()
        {
            MultipartFormDataContent content = new();
            content.Add(new JsonContent(this), "payload_json");
            if (Attachments != null)
            {
                var count = Attachments.Count;
                for (var i = 0; i < count; i++)
                {
                    MessageAttachment attachment = Attachments[i];
                    content.Add(new StreamContent(attachment.Stream), $"files[{i}]", attachment.FileName);
                }
            }
            return content;
        }
    }
}
