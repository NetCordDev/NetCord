using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class PremiumButtonProperties(ulong skuId) : IButtonProperties
{
    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; } = skuId;

    [JsonPropertyName("style")]
    public ButtonStyle Style => (ButtonStyle)6;

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Button;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }
}
