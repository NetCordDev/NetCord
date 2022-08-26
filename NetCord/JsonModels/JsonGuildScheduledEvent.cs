using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildScheduledEvent : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }

    [JsonPropertyName("creator_id")]
    public Snowflake? CreatorId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("scheduled_start_time")]
    public DateTimeOffset ScheduledStartTime { get; init; }

    [JsonPropertyName("scheduled_end_time")]
    public DateTimeOffset? ScheduledEndTime { get; init; }

    [JsonPropertyName("privacy_level")]
    public GuildScheduledEventPrivacyLevel PrivacyLevel { get; init; }

    [JsonPropertyName("status")]
    public GuildScheduledEventStatus Status { get; init; }

    [JsonPropertyName("entity_type")]
    public GuildScheduledEventEntityType EntityType { get; init; }

    [JsonPropertyName("entity_id")]
    public Snowflake? EntityId { get; init; }

    [JsonPropertyName("entity_metadata")]
    public JsonGuildScheduledEventMetadata? EntityMetadata { get; init; }

    [JsonPropertyName("creator")]
    public JsonUser? Creator { get; init; }

    [JsonPropertyName("user_count")]
    public int? UserCount { get; init; }
}
