using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonAuthorizationInformation
{
    [JsonPropertyName("application")]
    public JsonApplication Application { get; set; }

    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }

    [JsonPropertyName("expires")]
    public DateTimeOffset ExpiresAt { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }
}
