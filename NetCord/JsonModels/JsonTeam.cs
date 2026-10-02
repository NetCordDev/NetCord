using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonTeam : JsonEntity
{
    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("members")]
    public required JsonTeamUser[] Users { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("owner_user_id")]
    public required ulong OwnerId { get; set; }
}
