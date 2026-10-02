using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonGateway
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}
