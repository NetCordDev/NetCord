using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonRestInviteChannel : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public ChannelType Type { get; set; }
}

[JsonGuard]
public partial class JsonRestInvite
{
    [JsonPropertyName("type")]
    public InviteType Type { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("guild")]
    public JsonPartialGuild? Guild { get; set; }

    [JsonPropertyName("channel")]
    public JsonRestInviteChannel? Channel { get; set; }

    [JsonPropertyName("inviter")]
    public JsonUser? Inviter { get; set; }

    [JsonPropertyName("target_type")]
    public InviteTargetType? TargetType { get; set; }

    [JsonPropertyName("target_user")]
    public JsonUser? TargetUser { get; set; }

    [JsonPropertyName("target_application")]
    public JsonPartialApplication? TargetApplication { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public int? ApproximateUserCount { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("guild_scheduled_event")]
    public JsonGuildScheduledEvent? GuildScheduledEvent { get; set; }

    [JsonPropertyName("flags")]
    public InviteFlags? Flags { get; set; }

    [JsonPropertyName("roles")]
    public JsonRole[]? Roles { get; set; }

    [JsonPropertyName("uses")]
    public int? Uses { get; set; }

    [JsonPropertyName("max_uses")]
    public int? MaxUses { get; set; }

    [JsonPropertyName("max_age")]
    public int? MaxAge { get; set; }

    [JsonPropertyName("temporary")]
    public bool? Temporary { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }
}
