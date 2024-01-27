using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildUserInfo
{
    [JsonPropertyName("member")]
    public JsonGuildUser User { get; set; }

    [JsonPropertyName("source_invite_code")]
    public string? SourceInviteCode { get; set; }

    [JsonPropertyName("join_source_type")]
    public GuildUserJoinSourceType JoinSourceType { get; set; }

    [JsonPropertyName("inviter_id")]
    public ulong? InviterId { get; set; }
}
