using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonEmoji
{
    [JsonPropertyName("id")]
    public DiscordId? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("roles")]
    public JsonGuildRole[] AllowedRoles { get; init; }

    [JsonPropertyName("user")]
    public JsonUser? Creator { get; init; }

    [JsonPropertyName("require_colons")]
    public bool? RequireColons { get; init; }

    [JsonPropertyName("managed")]
    public bool? Managed { get; init; }

    [JsonPropertyName("animated")]
    public bool Animated { get; init; }

    [JsonPropertyName("available")]
    public bool? Available { get; init; }
}
