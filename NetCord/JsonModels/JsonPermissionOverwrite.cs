using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonPermissionOverwrite : JsonEntity
{
    [JsonPropertyName("type")]
    public PermissionOverwriteType Type { get; init; }

    [JsonPropertyName("allow")]
    public string Allowed { get; init; }

    [JsonPropertyName("deny")]
    public string Denied { get; init; }
}