using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents a modification to apply to a message.
/// </summary>
[GenerateMethodsForProperties]
public partial class MessageOptions : IHttpSerializable
{
    internal MessageOptions()
    {
    }

    /// <inheritdoc cref="IMessageProperties.Content"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <inheritdoc cref="IMessageProperties.Embeds"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("embeds")]
    public IEnumerable<EmbedProperties>? Embeds { get; set; }

    /// <inheritdoc cref="IMessageProperties.Flags"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("flags")]
    public MessageFlags? Flags { get; set; }

    /// <inheritdoc cref="IMessageProperties.AllowedMentions"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("allowed_mentions")]
    public AllowedMentionsProperties? AllowedMentions { get; set; }

    /// <inheritdoc cref="IMessageProperties.Components"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("components")]
    public IEnumerable<IMessageComponentProperties>? Components { get; set; }

    /// <inheritdoc cref="IMessageProperties.Attachments"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonConverters.AttachmentPropertiesIEnumerableConverter))]
    [JsonPropertyName("attachments")]
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }

    public HttpContent Serialize()
    {
        return IMessageProperties.Serialize(this, Serialization.Default.MessageOptions, Attachments);
    }
}
