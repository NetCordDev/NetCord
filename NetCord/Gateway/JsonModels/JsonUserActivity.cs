using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonUserActivity
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required UserActivityType Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonConverter(typeof(JsonConverters.MillisecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("timestamps")]
    public JsonUserActivityTimestamps? Timestamps { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("status_display_type")]
    public UserActivityStatusDisplayType? StatusDisplayType { get; set; }

    [JsonPropertyName("details")]
    public string? Details { get; set; }

    [JsonPropertyName("details_url")]
    public string? DetailsUrl { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("state_url")]
    public string? StateUrl { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("party")]
    public JsonUserActivityParty? Party { get; set; }

    [JsonPropertyName("assets")]
    public JsonUserActivityAssets? Assets { get; set; }

    [JsonPropertyName("secrets")]
    public JsonUserActivitySecrets? Secrets { get; set; }

    [JsonPropertyName("instance")]
    public bool? Instance { get; set; }

    [JsonPropertyName("flags")]
    public UserActivityFlags? Flags { get; set; }

    [JsonPropertyName("buttons")]
    public string[]? Buttons { get; set; }
}

public class JsonUserActivityTimestamps
{
    [JsonConverter(typeof(JsonConverters.MillisecondsNullableUnixDateTimeOffsetConverter))]
    [JsonPropertyName("start")]
    public DateTimeOffset? Start { get; set; }

    [JsonConverter(typeof(JsonConverters.MillisecondsNullableUnixDateTimeOffsetConverter))]
    [JsonPropertyName("end")]
    public DateTimeOffset? End { get; set; }
}

public class JsonUserActivityParty
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("size")]
    public long[]? Size { get; set; }
}

public class JsonUserActivityAssets
{
    [JsonPropertyName("large_image")]
    public string? LargeImageId { get; set; }

    [JsonPropertyName("large_text")]
    public string? LargeText { get; set; }

    [JsonPropertyName("small_image")]
    public string? SmallImageId { get; set; }

    [JsonPropertyName("small_text")]
    public string? SmallText { get; set; }
}

public class JsonUserActivitySecrets
{
    [JsonPropertyName("join")]
    public string? Join { get; set; }

    [JsonPropertyName("spectate")]
    public string? Spectate { get; set; }

    [JsonPropertyName("match")]
    public string? Match { get; set; }
}
