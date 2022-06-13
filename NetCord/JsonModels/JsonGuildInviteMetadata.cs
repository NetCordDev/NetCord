using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildInviteMetadata
{
    [JsonPropertyName("uses")]
    public int Uses { get; init; }

    [JsonPropertyName("max_uses")]
    public int MaxUses { get; init; }

    [JsonPropertyName("max_age")]
    public int MaxAge { get; init; }

    [JsonPropertyName("temporary")]
    public bool Temporary { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}