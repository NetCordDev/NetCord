using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEmoji
{
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("roles")]
    public ulong[]? AllowedRoles { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? Creator { get; set; }

    [JsonPropertyName("require_colons")]
    public bool? RequireColons { get; set; }

    [JsonPropertyName("managed")]
    public bool? Managed { get; set; }

    [JsonPropertyName("animated")]
    public bool Animated { get; set; }

    [JsonPropertyName("available")]
    public bool? Available { get; set; }
}
