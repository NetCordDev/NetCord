using System.Text.Json.Serialization;

using NetCord.Gateway;

namespace NetCord.Rest;

/// <summary>
/// A specialized message object, for use with <see cref="RestClient.ExecuteWebhookAsync(ulong, string, NetCord.Rest.WebhookMessageProperties, bool, ulong?, bool, NetCord.Rest.RestRequestProperties?, CancellationToken)"/>.
/// </summary>
[GenerateMethodsForProperties]
public partial class WebhookMessageProperties : IHttpSerializable, IMessageProperties
{
    /// <inheritdoc cref="RestMessage.Content"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Overrides the default username of the webhook.
    /// </summary>
    /// <remarks>
    /// Must follow the rules set for <see cref="Webhook.Name"/>.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Overrides the default avatar of the webhook.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Whether the message is a Text-To-Speech message.
    /// </summary>
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
    public IEnumerable<IMessageComponentProperties>? Components { get; set; }

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

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("applied_tags")]
    public IEnumerable<ulong>? AppliedTags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("poll")]
    public MessagePollProperties? Poll { get; set; }

    public HttpContent Serialize()
    {
        return IMessageProperties.Serialize(this, Serialization.Default.WebhookMessageProperties, Attachments);
    }

    public static implicit operator WebhookMessageProperties(string content) => new()
    {
        Content = content
    };
}
