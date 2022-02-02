using System.Text.Json.Serialization;

namespace NetCord;

public class GuildInviteProperties
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
    public GuildInviteTargetType? TargetType { get; set; }

    [JsonPropertyName("target_user_id")]
    public DiscordId? TargetUserId { get; set; }

    [JsonPropertyName("target_application_id")]
    public DiscordId? TargetApplicationId { get; set; }
}