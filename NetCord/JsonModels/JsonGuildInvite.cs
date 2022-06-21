using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public record JsonGuildInvite
{
    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; init; }

    [JsonPropertyName("code")]
    public string Code { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("inviter")]
    public JsonUser? Inviter { get; init; }

    [JsonPropertyName("max_age")]
    public int MaxAge { get; init; }

    [JsonPropertyName("max_uses")]
    public int MaxUses { get; init; }

    [JsonPropertyName("target_type")]
    public GuildInviteTargetType? TargetType { get; init; }

    [JsonPropertyName("target_user")]
    public JsonUser? TargetUser { get; init; }

    [JsonPropertyName("target_application")]
    public JsonApplication? TargetApplication { get; init; }

    [JsonPropertyName("temporary")]
    public bool Temporary { get; init; }

    [JsonPropertyName("uses")]
    public int Uses { get; init; }
}