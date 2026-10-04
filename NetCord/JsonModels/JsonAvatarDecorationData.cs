using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonAvatarDecorationData
{
    [JsonPropertyName("asset")]
    public string Hash { get; set; }

    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }
}
