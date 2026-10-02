using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonInvite
{
    [JsonPropertyName("type")]
    public required InviteType Type { get; set; }

    [JsonPropertyName("channel_id")]
    public required ulong ChannelId { get; set; }

    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("inviter")]
    public JsonUser? Inviter { get; set; }

    [JsonPropertyName("max_age")]
    public required int MaxAge { get; set; }

    [JsonPropertyName("max_uses")]
    public required int MaxUses { get; set; }

    [JsonPropertyName("target_type")]
    public InviteTargetType? TargetType { get; set; }

    [JsonPropertyName("target_user")]
    public JsonUser? TargetUser { get; set; }

    [JsonPropertyName("target_application")]
    public JsonPartialApplication? TargetApplication { get; set; }

    [JsonPropertyName("temporary")]
    public required bool Temporary { get; set; }

    [JsonPropertyName("uses")]
    public required int Uses { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("role_ids")]
    public ulong[]? RoleIds { get; set; }
}
