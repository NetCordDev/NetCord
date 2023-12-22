using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class TestEntitlementProperties
{
    public TestEntitlementProperties(ulong skuId, ulong ownerId, TestEntitlementOwnerType ownerType)
    {
        SkuId = skuId;
        OwnerId = ownerId;
        OwnerType = ownerType;
    }

    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }

    [JsonPropertyName("owner_id")]
    public ulong OwnerId { get; set; }

    [JsonPropertyName("owner_type")]
    public TestEntitlementOwnerType OwnerType { get; set; }
}
