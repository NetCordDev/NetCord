using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public record JsonRestGuildInvite
{
    [JsonPropertyName("code")]
    public string Code { get; init; }

    [JsonPropertyName("guild")]
    public JsonGuild? Guild { get; init; }

    [JsonPropertyName("channel")]
    public JsonChannel? Channel { get; init; }

    [JsonPropertyName("inviter")]
    public JsonUser? Inviter { get; init; }

    [JsonPropertyName("target_type")]
    public GuildInviteTargetType? TargetType { get; init; }

    [JsonPropertyName("target_user")]
    public JsonUser? TargetUser { get; init; }

    [JsonPropertyName("target_application")]
    public JsonApplication? TargetApplication { get; init; }

    [JsonPropertyName("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; init; }

    [JsonPropertyName("approximate_member_count")]
    public int? ApproximateUserCount { get; init; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }

    [JsonPropertyName("stage_instance")]
    public JsonStageInstance? StageInstance { get; init; }

    [JsonPropertyName("guild_scheduled_event")]
    public JsonGuildScheduledEvent? GuildScheduledEvent { get; init; }

    [JsonPropertyName("metadata")]
    public JsonGuildInviteMetadata? Metadata { get; init; }
}
