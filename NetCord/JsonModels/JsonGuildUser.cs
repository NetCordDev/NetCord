using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildUser
{
    [JsonPropertyName("user")]
    public JsonUser User { get; init; }

    [JsonPropertyName("nick")]
    public string? Nickname { get; init; }

    [JsonPropertyName("avatar")]
    public string? GuildAvatarHash { get; init; }

    [JsonPropertyName("roles")]
    public IEnumerable<DiscordId> RolesIds { get; init; }

    [JsonPropertyName("hoisted_role")]
    public DiscordId? HoistedRoleId { get; init; }

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
}
