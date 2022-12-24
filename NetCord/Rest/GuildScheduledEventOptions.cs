using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildScheduledEventOptions
{
    internal GuildScheduledEventOptions()
    {
    }

    [JsonPropertyName("channel_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("entity_metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildScheduledEventMetadataProperties? Metadata { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("privacy_level")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildScheduledEventPrivacyLevel? PrivacyLevel { get; set; }

    [JsonPropertyName("scheduled_start_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? ScheduledStartTime { get; set; }

    [JsonPropertyName("scheduled_end_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? ScheduledEndTime { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    [JsonPropertyName("entity_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildScheduledEventEntityType? EntityType { get; set; }

    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildScheduledEventStatus? Status { get; set; }

    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ImageProperties? Image { get; set; }

    [JsonSerializable(typeof(GuildScheduledEventOptions))]
    public partial class GuildScheduledEventOptionsSerializerContext : JsonSerializerContext
    {
        public static GuildScheduledEventOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
