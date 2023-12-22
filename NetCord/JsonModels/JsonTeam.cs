using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonTeam : JsonEntity
{
    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("members")]
    public JsonTeamUser[] Users { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("owner_user_id")]
    public ulong OwnerId { get; set; }
}
