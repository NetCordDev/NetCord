using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonReadyApplication : JsonEntity
{
    [JsonPropertyName("flags")]
    public ApplicationFlags? Flags { get; set; }
}

public class JsonReadyEventArgs
{
    [JsonPropertyName("v")]
    public required ApiVersion Version { get; set; }

    [JsonPropertyName("user")]
    public required JsonUser User { get; set; }

    [JsonPropertyName("guilds")]
    public required JsonEntity[] Guilds { get; set; }

    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }

    [JsonPropertyName("resume_gateway_url")]
    public required string ResumeGatewayUrl { get; set; }

    [JsonPropertyName("shard")]
    public Shard? Shard { get; set; }

    [JsonPropertyName("application")]
    public required JsonReadyApplication Application { get; set; }
}
