using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonInvite
{
    [JsonPropertyName("type")]
    public InviteType Type { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("inviter")]
    public JsonUser? Inviter { get; set; }

    [JsonPropertyName("max_age")]
    public int MaxAge { get; set; }

    [JsonPropertyName("max_uses")]
    public int MaxUses { get; set; }

    [JsonPropertyName("target_type")]
    public InviteTargetType? TargetType { get; set; }

    [JsonPropertyName("target_user")]
    public JsonUser? TargetUser { get; set; }

    [JsonPropertyName("target_application")]
    public JsonApplication? TargetApplication { get; set; }

    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; }

    [JsonPropertyName("uses")]
    public int Uses { get; set; }
}
