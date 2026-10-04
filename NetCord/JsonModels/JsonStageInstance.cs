using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonStageInstance : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("topic")]
    public string Topic { get; set; }

    [JsonPropertyName("privacy_level")]
    public StageInstancePrivacyLevel PrivacyLevel { get; set; }

    [JsonPropertyName("discoverable_disabled")]
    public bool DiscoverableDisabled { get; set; }

    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong? GuildScheduledEventId { get; set; }
}
