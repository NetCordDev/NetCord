using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonTeam : JsonEntity
{
    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("members")]
    public JsonTeamUser[] Users { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("owner_user_id")]
    public ulong OwnerId { get; set; }

    [JsonSerializable(typeof(JsonTeam))]
    public partial class JsonTeamSerializerContext : JsonSerializerContext
    {
        public static JsonTeamSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
