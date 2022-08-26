using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonTeam : JsonEntity
{
    [JsonPropertyName("icon")]
    public string? Icon { get; init; }

    [JsonPropertyName("users")]
    public JsonTeamUser[] Users { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("owner_user_id")]
    public Snowflake OwnerId { get; init; }
}

public record JsonTeamUser
{
    [JsonPropertyName("membership_state")]
    public MembershipState MembershipState { get; init; }

    [JsonPropertyName("permissions")]
    public string[] Permissions { get; init; }

    [JsonPropertyName("team_id")]
    public Snowflake TeamId { get; init; }

    [JsonPropertyName("user")]
    public JsonUser User { get; init; }
}
