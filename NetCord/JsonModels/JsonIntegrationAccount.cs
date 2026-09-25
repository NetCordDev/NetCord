using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonIntegrationAccount
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
