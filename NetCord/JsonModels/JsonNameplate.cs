using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonNameplate
{
    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }

    [JsonPropertyName("asset")]
    public string Asset { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("palette")]
    public string Palette { get; set; }
}
