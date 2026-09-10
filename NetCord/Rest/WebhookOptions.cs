using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents a modification to perform on a <see cref="Webhook"/> object.
/// </summary>
[GenerateMethodsForProperties]
public partial class WebhookOptions
{
    internal WebhookOptions()
    {
    }

    /// <inheritdoc cref="Webhook.Name"/>c
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <inheritdoc cref="WebhookProperties.Avatar"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }

    /// <summary>
    /// The new channel ID to move the webhook to.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }
}
