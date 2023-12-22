using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonRestGuildInvite
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("guild")]
    public JsonGuild? Guild { get; set; }

    [JsonPropertyName("channel")]
    public JsonChannel? Channel { get; set; }

    [JsonPropertyName("inviter")]
    public JsonUser? Inviter { get; set; }

    [JsonPropertyName("target_type")]
    public GuildInviteTargetType? TargetType { get; set; }

    [JsonPropertyName("target_user")]
    public JsonUser? TargetUser { get; set; }

    [JsonPropertyName("target_application")]
    public JsonApplication? TargetApplication { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public int? ApproximateUserCount { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("stage_instance")]
    public JsonStageInstance? StageInstance { get; set; }

    [JsonPropertyName("guild_scheduled_event")]
    public JsonGuildScheduledEvent? GuildScheduledEvent { get; set; }

    [JsonPropertyName("metadata")]
    public JsonRestGuildInviteMetadata? Metadata { get; set; }
}
