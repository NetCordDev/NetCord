using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonStageInstance : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; init; }

    [JsonPropertyName("topic")]
    public string Topic { get; init; }

    [JsonPropertyName("privacy_level")]
    public StageInstancePrivacyLevel PrivacyLevel { get; init; }

    [JsonPropertyName("discoverable_disabled")]
    public bool DiscoverableDisabled { get; init; }
}
