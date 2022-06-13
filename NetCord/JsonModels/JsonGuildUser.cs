using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildUser
{
    [JsonPropertyName("user")]
    public JsonUser User { get; init; }

    [JsonPropertyName("nick")]
    public string? Nickname { get; init; }

    [JsonPropertyName("avatar")]
    public string? GuildAvatarHash { get; init; }

    [JsonPropertyName("roles")]
    public IEnumerable<Snowflake> RoleIds { get; init; }

    [JsonPropertyName("hoisted_role")]
    public Snowflake? HoistedRoleId { get; init; }

    [JsonPropertyName("joined_at")]
    public DateTimeOffset JoinedAt { get; init; }

    [JsonPropertyName("premium_since")]
    public DateTimeOffset? GuildBoostStart { get; init; }

    [JsonPropertyName("deaf")]
    public bool Deafened { get; init; }

    [JsonPropertyName("mute")]
    public bool Muted { get; init; }

    [JsonPropertyName("pending")]
    public bool? IsPending { get; init; }

    [JsonPropertyName("permissions")]
    public string? Permissions { get; init; }

    [JsonPropertyName("communication_disabled_until")]
    public DateTimeOffset? TimeOutUntil { get; init; }
}
