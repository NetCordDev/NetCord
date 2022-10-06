using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MessageOptions
{
    internal MessageOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("embeds")]
    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("allowed_mentions")]
    public AllowedMentionsProperties? AllowedMentions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("components")]
    public IEnumerable<ComponentProperties>? Components { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonConverters.AttachmentPropertiesIEnumerableConverter))]
    [JsonPropertyName("attachments")]
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    internal HttpContent Build()
    {
        MultipartFormDataContent content = new()
        {
            { new JsonContent<MessageOptions>(this, MessageOptionsSerializerContext.WithOptions.MessageOptions), "payload_json" }
        };
        if (Attachments != null)
        {
            var i = 0;
            foreach (var attachment in Attachments)
            {
                content.Add(new StreamContent(attachment.Stream), $"files[{i}]", attachment.FileName);
                i++;
            }
        }
        return content;
    }

    [JsonSerializable(typeof(MessageOptions))]
    public partial class MessageOptionsSerializerContext : JsonSerializerContext
    {
        public static MessageOptionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
