using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildPreview : JsonEntity
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("splash")]
    public string? SplashHash { get; set; }

    [JsonPropertyName("discovery_splash")]
    public string? DiscoverySplashHash { get; set; }

    [JsonPropertyName("emojis")]
    public required JsonEmoji[] Emojis { get; set; }

    [JsonPropertyName("features")]
    public required string[] Features { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public required int ApproximateUserCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public required int ApproximatePresenceCount { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("stickers")]
    public required JsonSticker[] Stickers { get; set; }
}

