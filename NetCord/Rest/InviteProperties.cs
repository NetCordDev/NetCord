using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class InviteProperties
{
    [JsonPropertyName("max_age")]
    public int? MaxAge { get; set; }

    [JsonPropertyName("max_uses")]
    public int? MaxUses { get; set; }

    [JsonPropertyName("temporary")]
    public bool? Temporary { get; set; }

    [JsonPropertyName("unique")]
    public bool? Unique { get; set; }

    [JsonPropertyName("target_type")]
    public InviteTargetType? TargetType { get; set; }

    [JsonPropertyName("target_user_id")]
    public ulong? TargetUserId { get; set; }

    [JsonPropertyName("target_application_id")]
    public ulong? TargetApplicationId { get; set; }
}
