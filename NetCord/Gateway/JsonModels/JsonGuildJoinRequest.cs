using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels;

public class JsonGuildJoinRequest : JsonEntity
{
    [JsonPropertyName("application_status")]
    public GuildJoinRequestStatus ApplicationStatus { get; set; }

    //[JsonPropertyName("created_at")]
    //public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("last_seen")]
    public DateTimeOffset LastSeenAt { get; set; }

    [JsonPropertyName("rejection_reason")]
    public string? RejectionReason { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("form_responses")]
    public IReadOnlyList<JsonGuildJoinRequestFormResponse> FormResponses { get; set; }

    [JsonPropertyName("actioned_by_user")]
    public JsonUser ActionedByUser { get; set; }

    [JsonPropertyName("actioned_at")]
    public ulong ActionedAt { get; set; }
}
