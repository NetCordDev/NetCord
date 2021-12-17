using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonEmoji : JsonEntity
{
#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    public override DiscordId? Id { get; init; }
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("roles")]
    public JsonRole[] AllowedRoles { get; init; }

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
