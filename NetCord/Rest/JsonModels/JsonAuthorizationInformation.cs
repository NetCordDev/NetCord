using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonAuthorizationInformation
{
    [JsonPropertyName("application")]
    public required JsonPartialApplication Application { get; set; }

    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; set; }

    [JsonPropertyName("expires")]
    public required DateTimeOffset ExpiresAt { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }
}
