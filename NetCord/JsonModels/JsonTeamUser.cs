using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonTeamUser
{
    [JsonPropertyName("membership_state")]
    public required MembershipState MembershipState { get; set; }

    [JsonPropertyName("team_id")]
    public required ulong TeamId { get; set; }

    [JsonPropertyName("user")]
    public required JsonUser User { get; set; }

    [JsonPropertyName("role")]
    public required TeamRole Role { get; set; }
}
