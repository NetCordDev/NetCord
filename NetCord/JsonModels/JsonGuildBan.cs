using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildBan
{
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("user")]
    public JsonUser User { get; init; }
}