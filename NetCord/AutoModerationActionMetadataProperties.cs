using System.Text.Json.Serialization;

namespace NetCord;

[GenerateMethodsForProperties]
public partial class AutoModerationActionMetadataProperties
{
    /// <summary>
    /// The ID of the channel to which user content should be logged.
    /// </summary>
    /// <remarks>
    /// Required for an <see cref="AutoModerationActionType.SendAlertMessage"/> action.
    /// This must be an existing channel.
    /// </remarks>
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// The timeout duration, in seconds.
    /// </summary>
    /// <remarks>
    /// Required for an <see cref="AutoModerationActionType.Timeout"/> action.
    /// The maximum duration is 2,419,200 seconds (4 weeks).
    /// </remarks>
    [JsonPropertyName("duration_seconds")]
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// An additional explanation that will be shown to members whenever their message is blocked.
    /// </summary>
    /// <remarks>
    /// Only applies to an <see cref="AutoModerationActionType.BlockMessage"/> action.
    /// The maximum length is 150 characters.
    /// </remarks>
    [JsonPropertyName("custom_message")]
    public string? CustomMessage { get; set; }
}
