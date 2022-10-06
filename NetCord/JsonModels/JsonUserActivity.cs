using System.Text.Json.Serialization;

using NetCord.Gateway;

namespace NetCord.JsonModels;

public partial class JsonUserActivity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public UserActivityType Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonConverter(typeof(JsonConverters.MillisecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("timestamps")]
    public JsonUserActivityTimestamps Timestamps { get; set; }

    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; set; }

    [JsonPropertyName("details")]
    public string? Details { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("party")]
    public JsonParty? Party { get; set; }

    [JsonPropertyName("assets")]
    public JsonUserActivityAssets? Assets { get; set; }

    [JsonPropertyName("secrets")]
    public JsonUserActivitySecrets? Secrets { get; set; }

    [JsonPropertyName("instance")]
    public bool? Instance { get; set; }

    [JsonPropertyName("flags")]
    public UserActivityFlags? Flags { get; set; }

    [JsonPropertyName("buttons")]
    public string[] ButtonsLabels { get; set; }

    [JsonSerializable(typeof(JsonUserActivity))]
    public partial class JsonUserActivitySerializerContext : JsonSerializerContext
    {
        public static JsonUserActivitySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
