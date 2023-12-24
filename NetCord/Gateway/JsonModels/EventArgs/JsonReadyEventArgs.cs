using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonReadyEventArgs
{
    [JsonPropertyName("v")]
    public ApiVersion Version { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("guilds")]
    public JsonEntity[] Guilds { get; set; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }

    [JsonPropertyName("resume_gateway_url")]
    public string ResumeGatewayUrl { get; set; }

    [JsonPropertyName("shard")]
    public Shard? Shard { get; set; }

    [JsonPropertyName("application")]
    public JsonApplication? Application { get; set; }
}
