using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class PremiumButtonProperties(ulong skuId) : IButtonProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; } = skuId;

    [JsonPropertyName("style")]
    public ButtonStyle Style => (ButtonStyle)6;

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Button;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    public void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.PremiumButtonProperties);
    }
}
