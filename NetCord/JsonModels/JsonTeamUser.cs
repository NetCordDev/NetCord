using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonTeamUser
{
    [JsonPropertyName("membership_state")]
    public MembershipState MembershipState { get; set; }

    [JsonPropertyName("team_id")]
    public ulong TeamId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("role")]
    public TeamRole Role { get; set; }
}
