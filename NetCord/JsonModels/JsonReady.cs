using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonReady
{
    [JsonPropertyName("user")]
    public JsonUser User { get; init; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; init; }

    [JsonPropertyName("application")]
    public JsonApplication Application { get; init; }

    [JsonPropertyName("private_channels")]
    public JsonChannel[] DMChannels { get; init; }
}