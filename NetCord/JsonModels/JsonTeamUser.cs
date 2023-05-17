using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonTeamUser
{
    [JsonPropertyName("membership_state")]
    public MembershipState MembershipState { get; set; }

    [JsonPropertyName("permissions")]
    public string[] Permissions { get; set; }

    [JsonPropertyName("team_id")]
    public ulong TeamId { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonSerializable(typeof(JsonTeamUser))]
    public partial class JsonTeamUserSerializerContext : JsonSerializerContext
    {
        public static JsonTeamUserSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
