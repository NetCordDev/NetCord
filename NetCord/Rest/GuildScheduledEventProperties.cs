using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildScheduledEventProperties
{
    [JsonPropertyName("channel_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("entity_metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildScheduledEventMetadataProperties? Metadata { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("privacy_level")]
    public GuildScheduledEventPrivacyLevel PrivacyLevel { get; }

    [JsonPropertyName("scheduled_start_time")]
    public DateTimeOffset ScheduledStartTime { get; }

    [JsonPropertyName("scheduled_end_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? ScheduledEndTime { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    [JsonPropertyName("entity_type")]
    public GuildScheduledEventEntityType EntityType { get; }

    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImageProperties? Image { get; set; }

    public GuildScheduledEventProperties(string name, GuildScheduledEventPrivacyLevel privacyLevel, DateTimeOffset scheduledStartTime, GuildScheduledEventEntityType entityType)
    {
        Name = name;
        PrivacyLevel = privacyLevel;
        ScheduledStartTime = scheduledStartTime;
        EntityType = entityType;
    }

    [JsonSerializable(typeof(GuildScheduledEventProperties))]
    public partial class GuildScheduledEventPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildScheduledEventPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
