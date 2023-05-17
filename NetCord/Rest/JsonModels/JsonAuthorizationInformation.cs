using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public partial class JsonAuthorizationInformation
{
    [JsonPropertyName("application")]
    public JsonApplication Application { get; set; }

    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }

    [JsonPropertyName("expires")]
    public DateTimeOffset ExpiresAt { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }

    [JsonSerializable(typeof(JsonAuthorizationInformation))]
    public partial class JsonAuthorizationInformationSerializerContext : JsonSerializerContext
    {
        public static JsonAuthorizationInformationSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
