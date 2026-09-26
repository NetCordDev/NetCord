using System.Text.Json.Serialization;

using NetCord.Gateway;

namespace NetCord.Rest;

/// <summary>
/// A specialized message object, for use with executing webhooks.
/// </summary>
[GenerateMethodsForProperties]
public partial class WebhookMessageProperties : IHttpSerializable, IMessageProperties
{
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

    /// <summary>
    /// When set, creates a thread with the specified name.
    /// </summary>
    /// <remarks>
    /// Requires the webhook channel to be a <see cref="ForumGuildChannel"/> or <see cref="MediaForumGuildChannel"/>.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("thread_name")]
    public string? ThreadName { get; set; }

    /// <summary>
    /// When set, represents the tag IDs to apply to the created thread (via <see cref="ThreadName"/>).
    /// </summary>
    /// <remarks>
    /// Requires the webhook channel to be a <see cref="ForumGuildChannel"/> or <see cref="MediaForumGuildChannel"/>.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("applied_tags")]
    public IEnumerable<ulong>? AppliedTags { get; set; }

    /// <summary>
    /// When set, creates a poll with the specified configuration.
    /// </summary>
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
