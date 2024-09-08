using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAvatarDecorationData
{
    [JsonPropertyName("asset")]
    public string Hash { get; set; }

    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }
}
