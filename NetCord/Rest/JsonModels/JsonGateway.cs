using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonGateway
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
