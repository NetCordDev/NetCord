using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonTeam : JsonEntity
{
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("users")]
    public JsonTeamUser[] Users { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("owner_user_id")]
    public ulong OwnerId { get; set; }

    [JsonSerializable(typeof(JsonTeam))]
    public partial class JsonTeamSerializerContext : JsonSerializerContext
    {
        public static JsonTeamSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}

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
        public static JsonTeamUserSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
