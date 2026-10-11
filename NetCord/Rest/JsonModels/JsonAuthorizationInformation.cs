using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonAuthorizationInformation
{
    [JsonPropertyName("application")]
    public JsonPartialApplication Application { get; set; }

    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }

    [JsonPropertyName("expires")]
    public DateTimeOffset ExpiresAt { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }
}
