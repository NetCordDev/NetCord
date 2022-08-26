using System.Text.Json.Serialization;

using NetCord.Gateway;

namespace NetCord.JsonModels.EventArgs;

public record JsonReadyEventArgs
{
    [JsonPropertyName("v")]
    public GatewayVersion Version { get; init; }

    [JsonPropertyName("user")]
    public JsonUser User { get; init; }

    [JsonPropertyName("guilds")]
    public IEnumerable<JsonGuild> Guilds { get; init; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; init; }

    [JsonPropertyName("shard")]
    public Shard? Shard { get; init; }

    [JsonPropertyName("application")]
    public JsonApplication Application { get; init; }

    [JsonPropertyName("private_channels")]
    public JsonChannel[] DMChannels { get; init; }
}
