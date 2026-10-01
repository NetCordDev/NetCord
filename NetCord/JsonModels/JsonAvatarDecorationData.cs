using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAvatarDecorationData
{
    [JsonPropertyName("asset")]
    public required string Hash { get; set; }

    [JsonPropertyName("sku_id")]
    public required ulong SkuId { get; set; }
}
