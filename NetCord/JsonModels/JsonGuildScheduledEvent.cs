using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildScheduledEvent : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId? ChannelId { get; init; }

    [JsonPropertyName("creator_id")]
    public DiscordId? CreatorId { get; init; }

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
    public DiscordId? EntityId { get; init; }

    [JsonPropertyName("entity_metadata")]
    public JsonGuildScheduledEventMetadata? EntityMetadata { get; init; }

    [JsonPropertyName("creator")]
    public JsonUser? Creator { get; init; }

    [JsonPropertyName("user_count")]
    public int? UserCount { get; init; }
}