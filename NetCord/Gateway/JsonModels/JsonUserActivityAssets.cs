using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;

public class JsonUserActivityAssets
{
    [JsonPropertyName("large_image")]
    public string? LargeImageId { get; set; }

    [JsonPropertyName("large_text")]
    public string? LargeText { get; set; }

    [JsonPropertyName("small_image")]
    public string? SmallImageId { get; set; }

    [JsonPropertyName("small_text")]
    public string? SmallText { get; set; }
}
