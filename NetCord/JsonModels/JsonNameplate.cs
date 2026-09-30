using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonNameplate
{
    [JsonPropertyName("sku_id")]
    public required ulong SkuId { get; set; }

    [JsonPropertyName("asset")]
    public required string Asset { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("palette")]
    public required string Palette { get; set; }
}
