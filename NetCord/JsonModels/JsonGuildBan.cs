using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildBan
{
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("user")]
    public JsonUser User { get; init; }
}
