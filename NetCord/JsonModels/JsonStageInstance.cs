using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonStageInstance : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("topic")]
    public required string Topic { get; set; }

    [JsonPropertyName("privacy_level")]
    public required StageInstancePrivacyLevel PrivacyLevel { get; set; }

    [JsonPropertyName("discoverable_disabled")]
    public required bool DiscoverableDisabled { get; set; }

    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong? GuildScheduledEventId { get; set; }
}
