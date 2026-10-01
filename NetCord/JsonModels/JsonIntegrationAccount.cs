using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonIntegrationAccount
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }
}
