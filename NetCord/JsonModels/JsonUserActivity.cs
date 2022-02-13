using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonUserActivity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("type")]
    public UserActivityType Type { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonConverter(typeof(JsonConverters.MillisecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("timestamps")]
    public JsonUserActivityTimestamps Timestamps { get; init; }

    [JsonPropertyName("application_id")]
    public DiscordId? ApplicationId { get; init; }

    [JsonPropertyName("details")]
    public string? Details { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; init; }

    [JsonPropertyName("party")]
    public JsonParty? Party { get; init; }

    [JsonPropertyName("assets")]
    public JsonUserActivityAssets? Assets { get; init; }

    [JsonPropertyName("secrets")]
    public JsonUserActivitySecrets? Secrets { get; init; }

    [JsonPropertyName("instance")]
    public bool? Instance { get; init; }

    [JsonPropertyName("flags")]
    public int? Flags { get; init; }

    [JsonPropertyName("buttons")]
    public JsonUserActivityButton[] Buttons { get; init; }
}
