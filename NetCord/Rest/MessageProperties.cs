using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MessageProperties : IHttpSerializable, IMessageProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("nonce")]
    public NonceProperties? Nonce { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("tts")]
    public bool Tts { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.AttachmentPropertiesIEnumerableConverter))]
    [JsonPropertyName("attachments")]
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("embeds")]
    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("allowed_mentions")]
    public AllowedMentionsProperties? AllowedMentions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("message_reference")]
    public MessageReferenceProperties? MessageReference { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("components")]
    public IEnumerable<ComponentProperties>? Components { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("sticker_ids")]
    public IEnumerable<ulong>? StickerIds { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("poll")]
    public MessagePollProperties? Poll { get; set; }

    public HttpContent Serialize()
    {
        MultipartFormDataContent content = new()
        {
            { new JsonContent<MessageProperties>(this, Serialization.Default.MessageProperties), "payload_json" },
        };
        AttachmentProperties.AddAttachments(content, Attachments);
        return content;
    }

    public static implicit operator MessageProperties(string content) => new()
    {
        Content = content,
    };
}
