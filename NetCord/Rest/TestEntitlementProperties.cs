using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class TestEntitlementProperties(ulong skuId, ulong ownerId, TestEntitlementOwnerType ownerType)
{
    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; } = skuId;

    [JsonPropertyName("owner_id")]
    public ulong OwnerId { get; set; } = ownerId;

    [JsonPropertyName("owner_type")]
    public TestEntitlementOwnerType OwnerType { get; set; } = ownerType;
}
