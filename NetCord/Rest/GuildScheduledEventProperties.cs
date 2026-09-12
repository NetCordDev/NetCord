using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class GuildScheduledEventProperties(string name, GuildScheduledEventPrivacyLevel privacyLevel, DateTimeOffset scheduledStartTime, GuildScheduledEventEntityType entityType)
{
    /// <summary>
    /// The channel ID in which the scheduled event will be hosted, or <see langword="null"/> if the scheduled event is a <see cref="GuildScheduledEventEntityType.External"/> event.
    /// </summary>
    /// <remarks>
    /// Required for a <see cref="GuildScheduledEventEntityType.StageInstance"/> or <see cref="GuildScheduledEventEntityType.Voice"/> scheduled event.
    /// </remarks>
    [JsonPropertyName("channel_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? ChannelId { get; set; }

    /// <summary>
    /// Additional metadata for the scheduled event.
    /// </summary>
    /// <remarks>
    /// Required for a <see cref="GuildScheduledEventEntityType.External"/> scheduled event and must contain a non-null <see cref="GuildScheduledEventMetadataProperties.Location"/> value.
    /// Must be <see langword="null"/> for a <see cref="GuildScheduledEventEntityType.StageInstance"/> or <see cref="GuildScheduledEventEntityType.Voice"/> scheduled event.
    /// </remarks>
    [JsonPropertyName("entity_metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildScheduledEventMetadataProperties? Metadata { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("privacy_level")]
    public GuildScheduledEventPrivacyLevel PrivacyLevel { get; set; } = privacyLevel;

    [JsonPropertyName("scheduled_start_time")]
    public DateTimeOffset ScheduledStartTime { get; set; } = scheduledStartTime;

    /// <summary>
    /// The time when the scheduled event is scheduled to end.
    /// </summary>
    /// <remarks>
    /// Required for a <see cref="GuildScheduledEventEntityType.External"/> scheduled event.
    /// Must be <see langword="null"/> for a <see cref="GuildScheduledEventEntityType.StageInstance"/> or <see cref="GuildScheduledEventEntityType.Voice"/> scheduled event.
    /// </remarks>
    [JsonPropertyName("scheduled_end_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? ScheduledEndTime { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    /// <summary>
    /// The type of the scheduled event.
    /// </summary>
    /// <remarks>
    /// This determines the requirements for the <see cref="ChannelId"/>, <see cref="Metadata"/>, and <see cref="ScheduledEndTime"/> properties.
    /// </remarks>
    [JsonPropertyName("entity_type")]
    public GuildScheduledEventEntityType EntityType { get; set; } = entityType;

    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImageProperties? Image { get; set; }
}
