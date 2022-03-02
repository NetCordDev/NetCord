using System.Text.Json.Serialization;

namespace NetCord;

internal class IdentifyProperties
{
    public IdentifyProperties(string token)
    {
        Token = token;
    }

    [JsonPropertyName("token")]
    public string Token { get; }

    [JsonPropertyName("properties")]
    public ConnectionPropertiesProperties Properties => new();

    //[JsonPropertyName("compress")]
    //public bool? Compress { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("large_threshold")]
    public int? LargeThreshold { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("shard")]
    public ShardProperties? Shard { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("presence")]
    public PresenceProperties? Presence { get; set; }

    [JsonPropertyName("intents")]
    public GatewayIntent Intents { get; set; }
}

internal class ConnectionPropertiesProperties
{
    [JsonPropertyName("$os")]
    public string Os { get; } = "linux";

    [JsonPropertyName("$browser")]
    public string Browser { get; } = "NetCord";

    [JsonPropertyName("$device")]
    public string Device { get; } = "NetCord";
}