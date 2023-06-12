using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class InteractionMessageProperties : IHttpSerializable
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("tts")]
    public bool Tts { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("embeds")]
    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("allowed_mentions")]
    public AllowedMentionsProperties? AllowedMentions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("components")]
    public IEnumerable<ComponentProperties>? Components { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.AttachmentPropertiesIEnumerableConverter))]
    [JsonPropertyName("attachments")]
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    public HttpContent Serialize()
    {
        MultipartFormDataContent content = new()
        {
            { new JsonContent<InteractionMessageProperties>(this, InteractionMessagePropertiesSerializerContext.WithOptions.InteractionMessageProperties), "payload_json" }
        };
        var attachments = Attachments;
        if (attachments is not null)
        {
            var i = 0;
            foreach (var attachment in attachments)
            {
                if (attachment is not GoogleCloudPlatformAttachmentProperties)
                    content.Add(new StreamContent(attachment.Stream!), $"files[{i}]", attachment.FileName);
                i++;
            }
        }
        return content;
    }

    public static implicit operator InteractionMessageProperties(string content) => new()
    {
        Content = content
    };

    [JsonSerializable(typeof(InteractionMessageProperties))]
    public partial class InteractionMessagePropertiesSerializerContext : JsonSerializerContext
    {
        public static InteractionMessagePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
