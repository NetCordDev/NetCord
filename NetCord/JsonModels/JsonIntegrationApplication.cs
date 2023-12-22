using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonIntegrationApplication : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("bot")]
    public JsonUser? Bot { get; set; }
}
