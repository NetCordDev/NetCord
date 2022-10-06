using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class WebhookMessageProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("tts")]
    public bool Tts { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("embeds")]
    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("allowed_mentions")]
    public AllowedMentionsProperties? AllowedMentions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("components")]
    public IEnumerable<ComponentProperties>? Components { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.AttachmentPropertiesIEnumerableConverter))]
    [JsonPropertyName("attachments")]
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("thread_name")]
    public string? ThreadName { get; set; }

    internal HttpContent Build()
    {
        MultipartFormDataContent content = new()
        {
            { new JsonContent<WebhookMessageProperties>(this, WebhookMessagePropertiesSerializerContext.WithOptions.WebhookMessageProperties), "payload_json" }
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

    public static implicit operator WebhookMessageProperties(string content) => new()
    {
        Content = content
    };

    [JsonSerializable(typeof(WebhookMessageProperties))]
    public partial class WebhookMessagePropertiesSerializerContext : JsonSerializerContext
    {
        public static WebhookMessagePropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
