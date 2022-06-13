using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonUserActivityAssets
{
    [JsonPropertyName("large_image")]
    public string? LargeImageId { get; init; }

    [JsonPropertyName("large_text")]
    public string? LargeText { get; init; }

    [JsonPropertyName("small_image")]
    public string? SmallImageId { get; init; }

    [JsonPropertyName("small_text")]
    public string? SmallText { get; init; }
}
