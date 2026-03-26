using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class InteractionMessageProperties : IHttpSerializable, IMessageProperties
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
    public IEnumerable<IMessageComponentProperties>? Components { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(JsonConverters.AttachmentPropertiesIEnumerableConverter))]
    [JsonPropertyName("attachments")]
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("poll")]
    public MessagePollProperties? Poll { get; set; }

    public HttpContent Serialize()
    {
        return IMessageProperties.Serialize(this, Serialization.Default.InteractionMessageProperties, Attachments);
    }

    public static implicit operator InteractionMessageProperties(string content) => new()
    {
        Content = content
    };
}
