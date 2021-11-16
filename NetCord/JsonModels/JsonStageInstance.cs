using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonStageInstance : JsonEntity
{
    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; init; }

    [JsonPropertyName("topic")]
    public string Topic { get; init; }

    [JsonPropertyName("privacy_level")]
    public StageInstancePrivacyLevel PrivacyLevel { get; init; }

    [JsonPropertyName("discoverable_disabled")]
    public bool DiscoverableDisabled { get; init; }
}
