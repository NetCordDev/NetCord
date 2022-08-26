using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonIntegrationApplication : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("summary")]
    public string Summary { get; init; }

    [JsonPropertyName("bot")]
    public JsonUser? Bot { get; init; }
}
